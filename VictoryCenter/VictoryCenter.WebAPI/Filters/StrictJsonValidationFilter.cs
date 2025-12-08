using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VictoryCenter.WebAPI.Filters;

public class StrictJsonValidationFilter : IAsyncResourceFilter
{
    private static readonly JsonSerializerOptions StrictOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        AllowTrailingCommas = false,
        PropertyNameCaseInsensitive = true
    };

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var contentType = context.HttpContext.Request.ContentType;

        if (contentType is null || !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        context.HttpContext.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.HttpContext.Request.Body,
            Encoding.UTF8,
            leaveOpen: true);

        var json = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(json))
        {
            await next();
            return;
        }

        var actionDescriptor = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        if (actionDescriptor is null)
        {
            await next();
            return;
        }

        foreach (var parameter in actionDescriptor.Parameters)
        {
            if (!ShouldValidateParameter(parameter))
            {
                continue;
            }

            var validationError = ValidateJson(json, parameter.ParameterType);
            if (validationError is not null)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError(string.Empty, $"Invalid JSON: {validationError}");
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState));
                return;
            }
        }

        await next();
    }

    private static bool ShouldValidateParameter(ParameterDescriptor parameter)
    {
        return parameter.ParameterType.IsClass &&
               parameter.ParameterType != typeof(string) &&
               !parameter.ParameterType.IsAbstract;
    }

    private static string? ValidateJson(string json, Type targetType)
    {
        var options = new JsonSerializerOptions
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            AllowTrailingCommas = false,
            PropertyNameCaseInsensitive = true
        };

        try
        {
            JsonSerializer.Deserialize(json, targetType, StrictOptions);
            return null;
        }
        catch (JsonException ex)
        {
            return ex.Message;
        }
    }
}
