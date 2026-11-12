using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Auth.Logout;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.RefreshTokenSessions;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class LogoutTests
{
    private readonly LogoutCommandHandler _commandHandler;
    private readonly Mock<UserManager<AdminUser>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IRefreshTokenSessionRepository> _mockRefreshTokenSessionRepository;

    public LogoutTests()
    {
        _mockUserManager = new Mock<UserManager<AdminUser>>(
            new Mock<IUserStore<AdminUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<AdminUser>>().Object,
            new IUserValidator<AdminUser>[0],
            new IPasswordValidator<AdminUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<AdminUser>>>().Object);

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockRefreshTokenSessionRepository = new Mock<IRefreshTokenSessionRepository>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockRepositoryWrapper.Setup(x => x.RefreshTokenSessionRepository)
            .Returns(_mockRefreshTokenSessionRepository.Object);

        _commandHandler = new LogoutCommandHandler(
            _mockHttpContextAccessor.Object,
            _mockUserManager.Object,
            _mockRepositoryWrapper.Object,
            TimeProvider.System);
    }

    [Fact]
    public async Task Handle_NoEmailInContext_ReturnsUnauthorized()
    {
        var cmd = new LogoutCommand();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal());
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsNotFound()
    {
        var cmd = new LogoutCommand();
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync((AdminUser?)null);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.AdminWithGivenEmailWasNotFound, result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(It.IsAny<AdminUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateFails_ReturnsNotUpdated()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { Id = 1, Email = "admin@gmail.com" };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(new Mock<IRequestCookieCollection>().Object);
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = AuthConstants.NotUpdated }));
        _mockRepositoryWrapper.Setup(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.NotUpdated, result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(admin), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.Null(admin.RefreshToken);
        Assert.Null(admin.RefreshTokenValidTo);
    }

    [Fact]
    public async Task Handle_ValidData_SucceedsAndClearsCookies()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { Id = 1, RefreshToken = "refresh_token", RefreshTokenValidTo = DateTimeOffset.UtcNow.AddDays(1) };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "admin@gmail.com") }));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateSecurityStampAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);
        Assert.Null(admin.RefreshToken);
        Assert.Null(admin.RefreshTokenValidTo);
        _mockRefreshTokenSessionRepository.Verify(x => x.RevokeAdminSessionsAsync(admin.Id, "Logout", It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(admin), Times.Once);
        mockResponseCookies.Verify(
            c => c.Delete(
                It.Is<string>(s => s == AuthConstants.RefreshTokenCookieName),
                It.Is<CookieOptions>(options => options.Path == AuthConstants.RefreshTokenCookiePath)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidData_RevokesAdminSessionsForCurrentAdmin()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { Id = 42, Email = "admin@gmail.com" };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateSecurityStampAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockRefreshTokenSessionRepository.Verify(
            x => x.RevokeAdminSessionsAsync(
                42,
                "Logout",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_GivenConcurrencyFailureOnUpdateSecurityStamp_RetriesWithFreshAdminAndSucceeds()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { Id = 1, Email = "admin@gmail.com" };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(new Mock<IRequestCookieCollection>().Object);
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.SetupSequence(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = new IdentityErrorDescriber().ConcurrencyFailure().Code,
                Description = "Optimistic concurrency failure"
            }))
            .ReturnsAsync(IdentityResult.Success);
        _mockRepositoryWrapper.Setup(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(admin), Times.Exactly(2));
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Once);
        _mockRefreshTokenSessionRepository.Verify(
            x => x.RevokeAdminSessionsAsync(
                admin.Id,
                "Logout",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
        mockResponseCookies.Verify(
            c => c.Delete(
                It.Is<string>(s => s == AuthConstants.RefreshTokenCookieName),
                It.Is<CookieOptions>(options => options.Path == AuthConstants.RefreshTokenCookiePath)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_GivenConcurrencyFailureOnBothAttempts_ReloadsAndPersistsRevocationsAndReturnsNotUpdated()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { Id = 1, Email = "admin@gmail.com" };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(new Mock<IRequestCookieCollection>().Object);
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.SetupSequence(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = new IdentityErrorDescriber().ConcurrencyFailure().Code,
                Description = "Optimistic concurrency failure"
            }))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = new IdentityErrorDescriber().ConcurrencyFailure().Code,
                Description = "Optimistic concurrency failure"
            }));
        _mockRepositoryWrapper.Setup(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.NotUpdated, result.Errors[0].Message);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(admin), Times.Exactly(2));
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Exactly(2));
        _mockRefreshTokenSessionRepository.Verify(
            x => x.RevokeAdminSessionsAsync(
                admin.Id,
                "Logout",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        mockResponseCookies.Verify(
            c => c.Delete(
                It.Is<string>(s => s == AuthConstants.RefreshTokenCookieName),
                It.Is<CookieOptions>(options => options.Path == AuthConstants.RefreshTokenCookiePath)),
            Times.Once);
    }
}
