using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
    public async Task Invoke_NoException_ShouldCallNextWithoutLogging()
    {
        //Arrange
        var context = new DefaultHttpContext();
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
        await middleware.Invoke(context);

        //Assert
        Assert.True(nextCalled);
        _loggerMock.Verify(
            m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Invoke_WithException_ShouldLogException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new NotImplementedException();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            _loggerMock.Object,
            _factoryMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(501, context.Response.StatusCode);

        _loggerMock.Verify(
            m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(typeof(ValidationException), 400, "Bad Request")]
    [InlineData(typeof(UnauthorizedAccessException), 401, "Unauthorized")]
    [InlineData(typeof(SecurityTokenException), 403, "Forbidden")]
    [InlineData(typeof(KeyNotFoundException), 404, "Not Found")]
    [InlineData(typeof(DbUpdateException), 409, "Conflict")]
    [InlineData(typeof(NotImplementedException), 501, "Not Implemented")]
    [InlineData(typeof(Exception), 500, "Internal Server Error")]
    public async Task HandleExceptionAsync_SetsCorrectStatusAndProblemDetails(
    Type exceptionType, int expectedStatusCode, string expectedTitle)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/test";

        var exception = (Exception)Activator.CreateInstance(exceptionType);

        var expectedProblemDetails = new ProblemDetails
        {
            Status = expectedStatusCode,
            Title = expectedTitle,
            Detail = exception.Message,
            Type = "test",
            Instance = "/test"
        };

        _factoryMock.Setup(f => f.CreateProblemDetails(
                context,
                expectedStatusCode,
                expectedTitle,
                It.IsAny<string>(),
                exception.Message,
                "/test"
            )).Returns(expectedProblemDetails);

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.CompletedTask,
            _loggerMock.Object,
            _factoryMock.Object);

        var method = typeof(ExceptionHandlingMiddleware)
            .GetMethod("HandleExceptionAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        await (Task)method.Invoke(middleware, [context, exception]);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var result = JsonSerializer.Deserialize<ProblemDetails>(
            responseJson// , new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(expectedProblemDetails.Title, result!.Title);
        Assert.Equal(expectedProblemDetails.Type, result.Type);
        Assert.Equal(expectedProblemDetails.Detail, result.Detail);
    }
}
