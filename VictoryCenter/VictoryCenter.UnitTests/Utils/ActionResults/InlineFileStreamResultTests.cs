using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Moq;
using VictoryCenter.WebAPI.Utils.ActionResults;

namespace VictoryCenter.UnitTests.Utils.ActionResults;

public class InlineFileStreamResultTests
{
    [Fact]
    public async Task ExecuteResultAsync_ShouldSetHeadersAndCallBaseExecutor()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var contentType = "application/pdf";
        var fileName = "report_2024.pdf";

        var result = new InlineFileStreamResult(fileStream, contentType, fileName);

        var httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();

        var mockExecutor = new Mock<IActionResultExecutor<FileStreamResult>>();
        mockExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<ActionContext>(), It.IsAny<FileStreamResult>()))
            .Returns(Task.CompletedTask);

        services.AddSingleton(mockExecutor.Object);
        httpContext.RequestServices = services.BuildServiceProvider();

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        };

        // Act
        await result.ExecuteResultAsync(actionContext);

        // Assert
        var headers = httpContext.Response.Headers;

        Assert.True(headers.ContainsKey(HeaderNames.CacheControl));
        Assert.Equal("private, no-store", headers[HeaderNames.CacheControl].ToString());

        Assert.True(headers.ContainsKey(HeaderNames.ContentDisposition));
        var contentDispositionValue = headers[HeaderNames.ContentDisposition].ToString();
        Assert.Contains("inline", contentDispositionValue);
        Assert.Contains(fileName, contentDispositionValue);

        mockExecutor.Verify(
            x => x.ExecuteAsync(actionContext, result),
            Times.Once);
    }
}
