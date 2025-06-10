using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class RequestResponseLoggingMiddlewareTests
{
    private readonly Mock<ILogger<RequestResponseLoggingMiddleware>> _loggerMock;

    public RequestResponseLoggingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
    }

    private DefaultHttpContext CreateContext(int statusCode)
    {
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();
        ctx.Request.Method = "POST";
        ctx.Request.Path = "/endpoint";
        ctx.Request.QueryString = new QueryString("?q=1");
        ctx.Response.StatusCode = statusCode;
        return ctx;
    }

    [Theory]
    [InlineData(200, LogLevel.Information)]
    [InlineData(201, LogLevel.Information)]
    [InlineData(400, LogLevel.Warning)]
    [InlineData(401, LogLevel.Warning)]
    [InlineData(404, LogLevel.Warning)]
    [InlineData(500, LogLevel.Error)]
    public async Task InvokeAsync_LogsAtExpectedLevel(int statusCode, LogLevel expectedLevel)
    {
        // Arrange
        var context = CreateContext(statusCode);
        var middleware = new RequestResponseLoggingMiddleware(
            _ => Task.CompletedTask,
            _loggerMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);
        await context.Response.StartAsync();

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                expectedLevel,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}