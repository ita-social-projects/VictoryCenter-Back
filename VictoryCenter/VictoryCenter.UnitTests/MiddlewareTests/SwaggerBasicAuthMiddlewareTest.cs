using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class SwaggerBasicAuthMiddlewareTest
{
    private readonly Mock<ILogger<SwaggerBasicAuthMiddleware>> _loggerMock = new();

    private readonly Mock<UserManager<AdminUser>> _userManagerMock =
        new(
            Mock.Of<IUserStore<AdminUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<AdminUser>>(),
            Array.Empty<IUserValidator<AdminUser>>(),
            Array.Empty<IPasswordValidator<AdminUser>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<AdminUser>>>()
        );

    [Fact]
    public async Task InvokeAsync_NonSwaggerPath_CallsNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ValidCredentials_CallsNext()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("admin@test.com", "password123"));

        var admin = new AdminUser();
        var nextCalled = false;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("admin@test.com"))
            .ReturnsAsync(admin);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(admin, "password123"))
            .ReturnsAsync(true);

        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("admin@test.com", "wrongPassword"));

        var admin = new AdminUser();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("admin@test.com"))
            .ReturnsAsync(admin);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(admin, "wrongPassword"))
            .ReturnsAsync(false);

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Fact]
    public async Task InvokeAsync_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("unknown@test.com", "password123"));

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("unknown@test.com"))
            .ReturnsAsync((AdminUser?)null);

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);

        _userManagerMock.Verify(
            x => x.CheckPasswordAsync(
                It.IsAny<AdminUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_MissingAuthorizationHeader_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateContext("/swagger");

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Theory]
    [InlineData("Bearer token")]
    [InlineData("Digest credentials")]
    [InlineData("Basic")]
    [InlineData("Basic ")]
    public async Task InvokeAsync_InvalidAuthorizationHeader_ReturnsUnauthorized(
        string authorization)
    {
        // Arrange
        var context = CreateContext("/swagger", authorization);

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Fact]
    public async Task InvokeAsync_InvalidBase64_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateContext("/swagger", "Basic invalid!!!");

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Fact]
    public async Task InvokeAsync_CredentialsWithoutSeparator_ReturnsUnauthorized()
    {
        // Arrange
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes("admin@test.compassword123"));

        var context = CreateContext("/swagger", $"Basic {credentials}");

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Fact]
    public async Task InvokeAsync_ValidCredentialsWithColonInPassword_CallsNext()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("admin@test.com", "pass:word123"));

        var admin = new AdminUser();
        var nextCalled = false;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("admin@test.com"))
            .ReturnsAsync(admin);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(admin, "pass:word123"))
            .ReturnsAsync(true);

        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_SwaggerJsonPath_RequiresAuthentication()
    {
        // Arrange
        var context = CreateContext("/swagger/v1/swagger.json");

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        AssertUnauthorized(context);
    }

    [Fact]
    public async Task InvokeAsync_HttpRequest_ReturnsForbidden()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("admin@test.com", "password123"),
            isHttps: false);

        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal(
            StatusCodes.Status403Forbidden,
            context.Response.StatusCode);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_InvalidCredentials_LogsWarning()
    {
        // Arrange
        var context = CreateContext(
            "/swagger",
            CreateBasicAuthHeader("admin@test.com", "wrongPassword"));

        var admin = new AdminUser();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("admin@test.com"))
            .ReturnsAsync(admin);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(admin, "wrongPassword"))
            .ReturnsAsync(false);

        var middleware = CreateMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, _userManagerMock.Object);

        // Assert
        AssertUnauthorized(context);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("Swagger authentication failed")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private SwaggerBasicAuthMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new SwaggerBasicAuthMiddleware(next, _loggerMock.Object);
    }

    private static DefaultHttpContext CreateContext(
        string path,
        string? authorization = null,
        bool isHttps = true)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Scheme = isHttps ? "https" : "http";

        if (authorization is not null)
        {
            context.Request.Headers.Authorization = authorization;
        }

        return context;
    }

    private static string CreateBasicAuthHeader(
        string email,
        string password)
    {
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{email}:{password}"));

        return $"Basic {credentials}";
    }

    private static void AssertUnauthorized(HttpContext context)
    {
        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            context.Response.StatusCode);

        Assert.Equal(
            "Basic realm=\"Swagger Documentation\"",
            context.Response.Headers.WWWAuthenticate.ToString());
    }
}
