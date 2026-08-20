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
    public async Task InvokeAsync_ShouldReturn400WithJoinedErrors_WhenValidationExceptionThrown()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Amount", "Amount must be positive")
        };
        var validationException = new ValidationException(failures);
        var expectedDetail = "Name is required; Amount must be positive";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = expectedDetail
        };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                null,
                null,
                expectedDetail,
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

        var failure = new ValidationFailure("P", "E");
        var validationException = new ValidationException([failure]);

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                null,
                null,
                "E",
                null))
            .Returns(new ProblemDetails { Status = StatusCodes.Status400BadRequest });

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw validationException,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_RequestBodyTooLarge_ShouldReturn413WithoutCriticalLog()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new BadHttpRequestException(
            "Request body too large.",
            StatusCodes.Status413PayloadTooLarge);
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status413PayloadTooLarge,
            Title = "Payload Too Large",
            Detail = "The request body exceeds the allowed size."
        };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status413PayloadTooLarge,
                "Payload Too Large",
                null,
                "The request body exceeds the allowed size.",
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status413PayloadTooLarge, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_MultipleValidationErrors_ShouldJoinWithSemicolon()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("A", "Error A"),
            new("B", "Error B"),
            new("C", "Error C")
        };
        var validationException = new ValidationException(failures);
        var expectedDetail = "Error A; Error B; Error C";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = expectedDetail
        };

        _factoryMock
            .Setup(f => f.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                null,
                null,
                expectedDetail,
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw validationException,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = JsonSerializer.Deserialize<ProblemDetails>(
            await new StreamReader(context.Response.Body).ReadToEndAsync());

        Assert.NotNull(body);
        Assert.Equal(expectedDetail, body.Detail);
        Assert.Equal(StatusCodes.Status400BadRequest, body.Status);
    }
}
