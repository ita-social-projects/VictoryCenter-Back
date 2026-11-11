using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            var modelState = new ModelStateDictionary();
            foreach (var failure in validationException.Errors)
            {
                modelState.AddModelError(failure.PropertyName ?? string.Empty, failure.ErrorMessage);
            }

            var problemDetails = _problemsFactory.CreateValidationProblemDetails(
                context,
                modelState,
                statusCode: StatusCodes.Status400BadRequest);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (BadHttpRequestException badRequestException)
            when (badRequestException.StatusCode == StatusCodes.Status413PayloadTooLarge)
        {
            var problemDetails = _problemsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status413PayloadTooLarge,
                title: "Payload Too Large",
                detail: "The request body exceeds the allowed size.");

            context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                JsonSerializerOptions.Web,
                "application/problem+json",
                context.RequestAborted);
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
