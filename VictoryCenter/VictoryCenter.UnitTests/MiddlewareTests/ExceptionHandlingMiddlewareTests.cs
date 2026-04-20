using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly Mock<ProblemDetailsFactory> _factoryMock;

    public ExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new();
        _factoryMock = new();
    }

    [Fact]
    public async Task InvokeAsync_NoException_ShouldCallNextWithoutLoggingOrWritingResponse()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var nextCalled = false;

        var middleware = new ExceptionHandlingMiddleware(
            ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        Assert.True(string.IsNullOrEmpty(await new StreamReader(context.Response.Body).ReadToEndAsync()));
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500AndLogCritical_WhenUnhandledExceptionThrown()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test";
        context.Response.Body = new MemoryStream();

        var exception = new NotImplementedException("exception message");
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An error occurred while processing your request. Please try again!"
        };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                null,
                "An error occurred while processing your request. Please try again!",
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<NotImplementedException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = JsonSerializer.Deserialize<ProblemDetails>(
            await new StreamReader(context.Response.Body).ReadToEndAsync());

        Assert.NotNull(body);
        Assert.Equal(problemDetails.Status, body.Status);
        Assert.Equal(problemDetails.Title, body.Title);
        Assert.Equal(problemDetails.Detail, body.Detail);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400WithErrors_WhenValidationExceptionThrown()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Amount", "Amount must be positive")
        };
        var validationException = new ValidationException(failures);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Type = "ValidationFailure",
            Detail = "One or more validation errors has occurred"
        };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Validation error",
                "ValidationFailure",
                "One or more validation errors has occurred",
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw validationException,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotLog_WhenValidationExceptionThrown()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Validation error",
                "ValidationFailure",
                "One or more validation errors has occurred",
                null))
            .Returns(new ProblemDetails { Status = StatusCodes.Status400BadRequest });

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ValidationException([new ValidationFailure("P", "E")]),
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncludeValidationErrors_InProblemDetailsExtensions()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failure = new ValidationFailure("Name", "Name is required");
        var validationException = new ValidationException([failure]);

        var problemDetails = new ProblemDetails { Status = StatusCodes.Status400BadRequest };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Validation error",
                "ValidationFailure",
                "One or more validation errors has occurred",
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw validationException,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        Assert.True(problemDetails.Extensions.ContainsKey("errors"));
        Assert.Equal(validationException.Errors, problemDetails.Extensions["errors"]);
    }
}
