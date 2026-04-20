using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace VictoryCenter.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _problemsFactory;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ProblemDetailsFactory problemsFactory)
    {
        _next = next;
        _logger = logger;
        _problemsFactory = problemsFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            var problemDetails = _problemsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Validation error",
                "ValidationFailure",
                "One or more validation errors has occurred");

            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            if (errors.Count > 0)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
        {
            _logger.LogCritical(
                exception,
                "Unhandled exception occured while processing request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path + context.Request.QueryString);

            var problem = _problemsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An error occurred while processing your request. Please try again!");

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status!.Value;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
