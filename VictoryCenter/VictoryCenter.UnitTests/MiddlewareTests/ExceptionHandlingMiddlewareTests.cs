using System.Text.Json;
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
        //Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var nextCalled = false;
        var nextDelegate = new RequestDelegate(ctx =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var middleware = new ExceptionHandlingMiddleware(
            nextDelegate,
            _loggerMock.Object,
            _factoryMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        //Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responceBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.True(string.IsNullOrEmpty(responceBody));
        _loggerMock.VerifyNoOtherCalls();

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithException_ShouldLogExceptionAndWriteProblemDetails()
    {
        // Arrange
        var expectedMethod = "GET";
        var expectedPath = "/test";

        var context = new DefaultHttpContext();
        context.Request.Method = expectedMethod;
        context.Request.Path = expectedPath;
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
                null
            ))
            .Returns(problemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            _loggerMock.Object,
            _factoryMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<NotImplementedException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );

        _factoryMock.Verify(
            f => f.CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                null,
                "An error occurred while processing your request. Please try again!",
                null
            ),
            Times.Once
        );

        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(500, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var deserializedBody = JsonSerializer.Deserialize<ProblemDetails>(responseBody);

        Assert.NotNull(deserializedBody);
        Assert.Equal(problemDetails.Status, deserializedBody.Status);
        Assert.Equal(problemDetails.Title, deserializedBody.Title);
        Assert.Equal(problemDetails.Detail, deserializedBody.Detail);
        Assert.Null(problemDetails.Instance);
        Assert.Null(problemDetails.Type);
    }
}