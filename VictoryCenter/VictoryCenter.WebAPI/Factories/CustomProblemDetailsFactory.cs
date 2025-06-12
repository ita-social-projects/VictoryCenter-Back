using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace VictoryCenter.WebAPI.Factories;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
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
            instance: instance
        );

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
                instance: instance
            );

        return validationProblemDetails;
    }

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