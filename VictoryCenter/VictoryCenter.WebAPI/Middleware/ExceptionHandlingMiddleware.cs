using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace VictoryCenter.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ProblemDetailsFactory _factory;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ProblemDetailsFactory factory)
    {
        _next = next;
        _logger = logger;
        _factory = factory;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Exception occured while processing request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode = GetProblemStatusCode(exception);

        var problemDetails = _factory.CreateProblemDetails(
            context,
            statusCode,
            GetProblemTitle(statusCode),
            GetProblemType(statusCode),
            exception.Message,
            context.Request.Path
        );

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static int GetProblemStatusCode(Exception exception)
    {
        HttpStatusCode statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ArgumentNullException => HttpStatusCode.BadRequest,
            FormatException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            SecurityTokenException => HttpStatusCode.Forbidden,
            KeyNotFoundException => HttpStatusCode.NotFound,
            DbUpdateException => HttpStatusCode.Conflict,
            SqlException => HttpStatusCode.InternalServerError,
            NotImplementedException => HttpStatusCode.NotImplemented,
            _ => HttpStatusCode.InternalServerError
        };

        return (int)statusCode;
    }

    private static string GetProblemType(int statusCode) =>
        statusCode switch
        {
            400 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request",
            401 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-401-unauthorized",
            403 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-403-forbidden",
            404 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-404-not-found",
            409 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-409-conflict",
            413 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-413-content-too-large",
            500 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error",
            501 => "https://www.rfc-editor.org/rfc/rfc9110.html#name-501-not-implemented",
            _ => "about:blank"
        };

    private static string GetProblemTitle(int statusCode) =>
        statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            413 => "Content Too Large",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            _ => "Unexpected Error"
        };
}
