using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        int status;
        string? title;
        string detail;
        switch (exception)
        {
            case ValidationException validationException:
                status = StatusCodes.Status400BadRequest;
                title = null;
                detail = string.Join("; ", validationException.Errors.Select(error => error.ErrorMessage));
                break;
            case BadHttpRequestException badRequestException when badRequestException.StatusCode == StatusCodes.Status413PayloadTooLarge:
                status = StatusCodes.Status413PayloadTooLarge;
                title = "Payload Too Large";
                detail = "The request body exceeds the allowed size.";
                break;
            default:
                status = StatusCodes.Status500InternalServerError;
                title = "Internal Server Error";
                detail = "An error occurred while processing your request. Please try again!";
                _logger.LogCritical(
                    exception,
                    "Unhandled exception occured while processing request: {Method} {Path}",
                    context.Request.Method,
                    _sanitizer.GetSafePathAndQuery(context.Request));
                break;
        }

        var problem = _problemsFactory.CreateProblemDetails(context, statusCode: status, title: title, detail: detail);
        context.Response.StatusCode = status;
        if (!await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem,
        }))
        {
            await context.Response.WriteAsJsonAsync(problem, JsonSerializerOptions.Web, "application/problem+json", cancellationToken);
        }

        return true;
    }
}
