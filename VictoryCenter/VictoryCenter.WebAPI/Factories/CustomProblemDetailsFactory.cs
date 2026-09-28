using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.WebAPI.Factories;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    private const string JsonPathPrefix = "$";
    private const string JsonPropertyPathPrefix = "$.";
    private const string UnknownBodyName = "RequestBody";

    private static readonly string[] NumericTypeNames =
        ["System.Decimal", "System.Double", "System.Single", "System.Int16", "System.Int32", "System.Int64"];

    private readonly DefaultProblemDetailsFactory _innerFactory;

    public CustomProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
    {
        _innerFactory = new DefaultProblemDetailsFactory(options);
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var code = statusCode ?? StatusCodes.Status400BadRequest;

        var problemDetails = _innerFactory.CreateProblemDetails(
            httpContext,
            code,
            title: title ?? GetDefaultTitle(code),
            type: type,
            detail: detail ?? GetDefaultDetail(code),
            instance: instance);

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var code = statusCode ?? StatusCodes.Status400BadRequest;

        var validationProblemDetails = _innerFactory
            .CreateValidationProblemDetails(
                httpContext,
                modelStateDictionary,
                code,
                title: title ?? GetDefaultTitle(code),
                type: type,
                detail: detail ?? GetDefaultDetail(code),
                instance: instance);

        FormatJsonConversionErrors(validationProblemDetails.Errors, GetBodyParameter(httpContext));

        return validationProblemDetails;
    }

    // System.Text.Json reports conversion errors under "$"-prefixed keys with raw framework messages
    // and the body DTO then gets an implicit "field is required" error because it stays null
    private static void FormatJsonConversionErrors(IDictionary<string, string[]> errors, ParameterDescriptor? bodyParameter)
    {
        var jsonPaths = errors.Keys.Where(key => key.StartsWith(JsonPathPrefix, StringComparison.Ordinal)).ToList();
        if (jsonPaths.Count == 0)
        {
            return;
        }

        if (bodyParameter is not null)
        {
            errors.Remove(bodyParameter.Name);
        }

        var bodyName = bodyParameter?.ParameterType.Name;

        foreach (var jsonPath in jsonPaths)
        {
            var rawMessages = errors[jsonPath];
            errors.Remove(jsonPath);

            if (!jsonPath.StartsWith(JsonPropertyPathPrefix, StringComparison.Ordinal))
            {
                var bodyKey = bodyName ?? UnknownBodyName;
                errors[bodyKey] = [ErrorMessagesConstants.PropertyMustBeInAValidFormat(bodyKey)];
                continue;
            }

            var propertyName = ToPropertyName(jsonPath);
            var key = bodyName is null ? propertyName : $"{bodyName}.{propertyName}";
            errors[key] = rawMessages.Select(message => GetFriendlyMessage(message, propertyName)).Distinct().ToArray();
        }
    }

    private static string ToPropertyName(string jsonPath) =>
        string.Join(
            '.',
            jsonPath[JsonPropertyPathPrefix.Length..]
                .Split('.')
                .Select(segment => char.ToUpperInvariant(segment[0]) + segment[1..]));

    private static string GetFriendlyMessage(string rawMessage, string propertyName) =>
        NumericTypeNames.Any(type => rawMessage.Contains(type, StringComparison.Ordinal))
            ? ErrorMessagesConstants.PropertyMustContainOnlyDigits(propertyName)
            : ErrorMessagesConstants.PropertyMustBeInAValidFormat(propertyName);

    private static ParameterDescriptor? GetBodyParameter(HttpContext httpContext) =>
        httpContext.GetEndpoint()?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>()?
            .Parameters
            .FirstOrDefault(parameter => parameter.BindingInfo?.BindingSource == BindingSource.Body);

    private static string GetDefaultTitle(int statusCode) =>
        statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            _ => "Internal Server Error"
        };

    private static string GetDefaultDetail(int statusCode) =>
        statusCode switch
        {
            400 => "One or more validation errors occurred.",
            401 => "Authentication is required to access this resource.",
            403 => "You do not have permission to access this resource.",
            404 => "Resource was not found.",
            409 => "A conflict occurred with the current state of the resource.",
            _ => "Please, try again."
        };
}
