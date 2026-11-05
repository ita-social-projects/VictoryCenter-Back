using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public async Task InvokeAsync_ShouldReturn400WithFieldGroupedErrors_WhenValidationExceptionThrown()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Amount", "Amount must be positive")
        };
        var validationException = new ValidationException(failures);

        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest
        };

        _factoryMock
            .Setup(f => f.CreateValidationProblemDetails(
                context,
                It.Is<ModelStateDictionary>(ms =>
                    (ms["Name"] !).Errors.Single().ErrorMessage == "Name is required" &&
                    (ms["Amount"] !).Errors.Single().ErrorMessage == "Amount must be positive"),
                StatusCodes.Status400BadRequest,
                null,
                null,
                null,
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
            .Setup(f => f.CreateValidationProblemDetails(
                context,
                It.IsAny<ModelStateDictionary>(),
                StatusCodes.Status400BadRequest,
                null,
                null,
                null,
                null))
            .Returns(new ValidationProblemDetails { Status = StatusCodes.Status400BadRequest });

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
        context.Response.Body.Position = 0;
        using JsonDocument responseBody = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal(StatusCodes.Status413PayloadTooLarge, responseBody.RootElement.GetProperty("status").GetInt32());
        Assert.False(responseBody.RootElement.TryGetProperty("Status", out _));
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvokeAsync_MultipleValidationErrors_ShouldGroupByPropertyName()
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

        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Errors =
            {
                ["A"] = ["Error A"],
                ["B"] = ["Error B"],
                ["C"] = ["Error C"]
            }
        };

        _factoryMock
            .Setup(f => f.CreateValidationProblemDetails(
                context,
                It.IsAny<ModelStateDictionary>(),
                StatusCodes.Status400BadRequest,
                null,
                null,
                null,
                null))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw validationException,
            _loggerMock.Object,
            _factoryMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = JsonSerializer.Deserialize<ValidationProblemDetails>(
            await new StreamReader(context.Response.Body).ReadToEndAsync());

        Assert.NotNull(body);
        Assert.Equal(StatusCodes.Status400BadRequest, body.Status);
        Assert.Equal(["Error A"], body.Errors["A"]);
        Assert.Equal(["Error B"], body.Errors["B"]);
        Assert.Equal(["Error C"], body.Errors["C"]);
    }
}
