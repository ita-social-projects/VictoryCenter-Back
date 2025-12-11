using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VictoryCenter.BLL.Helpers;

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

        var validationErrors = new List<string>();

        foreach (var parameter in actionDescriptor.Parameters)
        {
            if (!ShouldValidateParameter(parameter))
            {
                continue;
            }

            var errors = JsonValidationHelper.ValidateJsonAgainstType(json, parameter.ParameterType, StrictOptions);
            if (errors.Count > 0)
            {
                validationErrors.AddRange(errors);
            }
        }

        if (validationErrors.Count > 0)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid JSON format",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationErrors.Count == 1
                    ? validationErrors[0]
                    : "The request contains multiple JSON validation errors. See the 'errors' property for details.",
                Extensions =
                {
                    ["errors"] = validationErrors
                }
            };

            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }

    private static bool ShouldValidateParameter(ParameterDescriptor parameter)
    {
        return parameter.ParameterType.IsClass &&
               parameter.ParameterType != typeof(string) &&
               !parameter.ParameterType.IsAbstract &&
               parameter.BindingInfo?.BindingSource == BindingSource.Body;
    }
}
