using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.WebAPI.ErrorHandling;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;
    private readonly ProblemDetailsFactory _problemsFactory;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IRequestLogSanitizer _sanitizer;

    public ApiExceptionHandler(
        ILogger<ApiExceptionHandler> logger,
        ProblemDetailsFactory problemsFactory,
        IProblemDetailsService problemDetailsService,
        IRequestLogSanitizer sanitizer)
    {
        _logger = logger;
        _problemsFactory = problemsFactory;
        _problemDetailsService = problemDetailsService;
        _sanitizer = sanitizer;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted || (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested))
        {
            return false;
        }

        ProblemDetails problem;
        switch (exception)
        {
            case ValidationException validationException:
                var modelState = new ModelStateDictionary();
                foreach (var failure in validationException.Errors)
                {
                    modelState.AddModelError(failure.PropertyName ?? string.Empty, failure.ErrorMessage);
                }

                problem = _problemsFactory.CreateValidationProblemDetails(context, modelState, statusCode: StatusCodes.Status400BadRequest);
                break;
            case BadHttpRequestException badRequestException when badRequestException.StatusCode == StatusCodes.Status413PayloadTooLarge:
                problem = _problemsFactory.CreateProblemDetails(
                    context,
                    statusCode: StatusCodes.Status413PayloadTooLarge,
                    title: "Payload Too Large",
                    detail: "The request body exceeds the allowed size.");
                break;
            default:
                _logger.LogCritical(
                    exception,
                    "Unhandled exception occured while processing request: {Method} {Path}",
                    context.Request.Method,
                    _sanitizer.GetSafePathAndQuery(context.Request));
                problem = _problemsFactory.CreateProblemDetails(
                    context,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error",
                    detail: "An error occurred while processing your request. Please try again!");
                break;
        }

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        if (!await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem,
        }))
        {
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), JsonSerializerOptions.Web, "application/problem+json", cancellationToken);
        }

        return true;
    }
}
