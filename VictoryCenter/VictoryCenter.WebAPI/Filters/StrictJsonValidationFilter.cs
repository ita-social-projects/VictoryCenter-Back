using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.WebAPI.Filters;

public class StrictJsonValidationFilter : IAsyncResourceFilter
{
    private const string UnknownBodyName = "RequestBody";

    private static readonly JsonSerializerOptions StrictOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        AllowTrailingCommas = false,
        PropertyNameCaseInsensitive = true
    };

    private readonly ProblemDetailsFactory _problemsFactory;

    public StrictJsonValidationFilter(ProblemDetailsFactory problemsFactory)
    {
        _problemsFactory = problemsFactory;
    }

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
        string? bodyParameterName = null;

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
                bodyParameterName ??= parameter.ParameterType.Name;
            }
        }

        if (validationErrors.Count > 0)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in validationErrors)
            {
                modelState.AddModelError(bodyParameterName ?? UnknownBodyName, error);
            }

            var problemDetails = _problemsFactory.CreateValidationProblemDetails(
                context.HttpContext,
                modelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid JSON format");

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
