using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.WebAPI.ErrorHandling;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.WebAPI.Factories;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.UnitTests.ErrorHandlingTests;

public class ApiExceptionHandlerTests
{
    private readonly Mock<ILogger<ApiExceptionHandler>> _logger = new();
    private readonly Mock<IProblemDetailsService> _writer = new();

    [Theory]
    [InlineData("validation", 400, "Bad Request", "One or more validation errors occurred.")]
    [InlineData("empty-validation", 400, "Bad Request", "One or more validation errors occurred.")]
    [InlineData("payload", 413, "Payload Too Large", "The request body exceeds the allowed size.")]
    [InlineData("unexpected", 500, "Internal Server Error", "An error occurred while processing your request. Please try again!")]
    [InlineData("unrelated-cancellation", 500, "Internal Server Error", "An error occurred while processing your request. Please try again!")]
    [InlineData("other-bad-request", 500, "Internal Server Error", "An error occurred while processing your request. Please try again!")]
    public async Task TryHandleAsync_PreservesClassificationAndSafeProblemDetails(string scenario, int status, string title, string detail)
    {
        var context = CreateContext();
        var exception = scenario switch
        {
            "validation" => new ValidationException([new ValidationFailure("Name", "Name is required"), new ValidationFailure("Amount", "Amount must be positive")]),
            "empty-validation" => new ValidationException(Array.Empty<ValidationFailure>()),
            "payload" => (Exception)new BadHttpRequestException("private payload message", StatusCodes.Status413PayloadTooLarge),
            "unrelated-cancellation" => new OperationCanceledException("private cancellation message"),
            "other-bad-request" => new BadHttpRequestException("private bad request", StatusCodes.Status400BadRequest),
            _ => new InvalidOperationException("private server details"),
        };

        var handled = await CreateHandler().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(status, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        context.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal(status, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(title, json.RootElement.GetProperty("title").GetString());
        Assert.Equal(detail, json.RootElement.GetProperty("detail").GetString());
        if (scenario == "validation")
        {
            Assert.Equal("Name is required", json.RootElement.GetProperty("errors").GetProperty("Name")[0].GetString());
            Assert.Equal("Amount must be positive", json.RootElement.GetProperty("errors").GetProperty("Amount")[0].GetString());
        }

        Assert.Equal("test-trace", json.RootElement.GetProperty("traceId").GetString());
        Assert.False(json.RootElement.TryGetProperty("type", out _));
        Assert.DoesNotContain("private", json.RootElement.ToString());
        VerifyCriticalLogs(status == 500 ? Times.Once() : Times.Never());
    }

    [Fact]
    public async Task TryHandleAsync_ValidationFallback_PreservesMultipleErrorsPerFieldAndEmptyProperty()
    {
        var context = CreateContext();
        var exception = new ValidationException([
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Name", "Name is too short"),
            new ValidationFailure(string.Empty, "Invalid request"),
        ]);

        await CreateHandler().TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(context.Response.Body);
        var errors = json.RootElement.GetProperty("errors");
        Assert.Equal(2, errors.GetProperty("Name").GetArrayLength());
        Assert.Equal("Name is too short", errors.GetProperty("Name")[1].GetString());
        Assert.Equal("Invalid request", errors.GetProperty(string.Empty)[0].GetString());
        _logger.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TryHandleAsync_UnexpectedError_LogsSanitizedPathExactlyOnce()
    {
        var context = CreateContext();
        context.Request.QueryString = new QueryString("?ticket=secret-ticket&id=5");

        await CreateHandler().TryHandleAsync(context, new IOException("storage failure"), CancellationToken.None);

        _logger.Verify(
            logger => logger.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("ticket=[REDACTED]")
                    && state.ToString()!.Contains("id=5") && !state.ToString()!.Contains("secret-ticket")),
                It.IsAny<IOException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_WhenFrameworkWriterAccepts_DoesNotWriteFallback()
    {
        var context = CreateContext();
        ProblemDetailsContext? captured = null;
        _writer.Setup(writer => writer.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(value => captured = value)
            .ReturnsAsync(true);

        Assert.True(await CreateHandler().TryHandleAsync(context, new Exception("private"), CancellationToken.None));

        Assert.NotNull(captured);
        Assert.Same(context, captured.HttpContext);
        Assert.Equal(500, captured.ProblemDetails.Status);
        Assert.Equal(0, context.Response.Body.Length);
        _writer.Verify(writer => writer.TryWriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_WhenWriterThrows_PropagatesFailureWithoutRetryingResponse()
    {
        var context = CreateContext();
        var writeError = new IOException("connection lost");
        _writer.Setup(writer => writer.TryWriteAsync(It.IsAny<ProblemDetailsContext>())).ThrowsAsync(writeError);

        var actual = await Assert.ThrowsAsync<IOException>(async () =>
            await CreateHandler().TryHandleAsync(context, new Exception("original failure"), CancellationToken.None));

        Assert.Same(writeError, actual);
        Assert.Equal(0, context.Response.Body.Length);
    }

    [Fact]
    public async Task TryHandleAsync_WhenResponseStarted_DeclinesWithoutChangingOrLogging()
    {
        var context = CreateContext();
        var feature = new Mock<IHttpResponseFeature>();
        feature.SetupGet(response => response.HasStarted).Returns(true);
        context.Features.Set(feature.Object);

        Assert.False(await CreateHandler().TryHandleAsync(context, new Exception("late failure"), CancellationToken.None));

        _writer.VerifyNoOtherCalls();
        _logger.VerifyNoOtherCalls();
        feature.VerifySet(response => response.StatusCode = It.IsAny<int>(), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TryHandleAsync_ClientCancellation_DeclinesWithoutServerErrorLog(bool taskCanceled)
    {
        var context = CreateContext();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        context.RequestAborted = cancellation.Token;
        Exception exception = taskCanceled ? new TaskCanceledException() : new OperationCanceledException();

        Assert.False(await CreateHandler().TryHandleAsync(context, exception, cancellation.Token));

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal(0, context.Response.Body.Length);
        _writer.VerifyNoOtherCalls();
        _logger.VerifyNoOtherCalls();
    }

    [Fact]
    public void Registration_UsesSingletonHandlerAndOnlySuppressesDuplicateFrameworkLogs()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ProblemDetailsFactory>(new CustomProblemDetailsFactory(Options.Create(new ApiBehaviorOptions())));
        services.AddSingleton<IRequestLogSanitizer>(new RequestLogSanitizer(Options.Create(new LogRedactionOptions())));
        var logger = new Mock<ILogger>();
        logger.Setup(value => value.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        var loggerProvider = new Mock<ILoggerProvider>();
        loggerProvider.Setup(value => value.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        services.AddLogging(logging => logging.AddProvider(loggerProvider.Object));
        services.AddApiExceptionHandling();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        var handler = Assert.Single(provider.GetServices<IExceptionHandler>());
        Assert.IsType<ApiExceptionHandler>(handler);
        Assert.Same(handler, Assert.Single(provider.GetServices<IExceptionHandler>()));
        var logging = provider.GetRequiredService<ILoggerFactory>();
        Assert.False(logging.CreateLogger("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware").IsEnabled(LogLevel.Error));
        Assert.True(logging.CreateLogger<ApiExceptionHandler>().IsEnabled(LogLevel.Critical));
        Assert.True(logging.CreateLogger("Other.Application.Component").IsEnabled(LogLevel.Error));
    }

    private ApiExceptionHandler CreateHandler() => new(
        _logger.Object,
        new CustomProblemDetailsFactory(Options.Create(new ApiBehaviorOptions())),
        _writer.Object,
        new RequestLogSanitizer(Options.Create(new LogRedactionOptions())));

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext { TraceIdentifier = "test-trace" };
        context.Request.Method = "GET";
        context.Request.Path = "/test";
        context.Response.Body = new MemoryStream();
        return context;
    }

    private void VerifyCriticalLogs(Times times) => _logger.Verify(
        logger => logger.Log(
            LogLevel.Critical,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        times);
}
