using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Auth.Logout;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.RefreshTokenSessions;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class LogoutTests
{
    private const string AdminEmail = "admin@gmail.com";
    private const string RawRefreshToken = "raw_refresh_token";
    private const string HashedRefreshToken = "hashed_refresh_token";

    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;
    private readonly Mock<UserManager<AdminUser>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IRefreshTokenSessionRepository> _mockRefreshTokenSessionRepository;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IRequestCookieCollection> _mockRequestCookies;
    private readonly Mock<IResponseCookies> _mockResponseCookies;
    private readonly LogoutCommandHandler _commandHandler;

    public LogoutTests()
    {
        _mockUserManager = CreateUserManagerMock();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenSessionRepository = new Mock<IRefreshTokenSessionRepository>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockRepositoryWrapper
            .Setup(x => x.RefreshTokenSessionRepository)
            .Returns(_mockRefreshTokenSessionRepository.Object);
        _mockRepositoryWrapper
            .Setup(x => x.ReloadAsync(It.IsAny<AdminUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockHttpContext = new Mock<HttpContext>();
        _mockRequestCookies = new Mock<IRequestCookieCollection>();
        _mockResponseCookies = new Mock<IResponseCookies>();

        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(_mockRequestCookies.Object);

        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(_mockResponseCookies.Object);

        _mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal());

        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(_mockHttpContext.Object);

        _commandHandler = new LogoutCommandHandler(
            _mockHttpContextAccessor.Object,
            _mockUserManager.Object,
            _mockTokenService.Object,
            _mockRepositoryWrapper.Object,
            TimeProvider.System);
    }

    [Fact]
    public async Task Handle_ValidAccessTokenClaim_RevokesSessionsAndDeletesCookie()
    {
        var admin = CreateAdmin(id: 42);
        SetupAccessTokenUser(AdminEmail);
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateSecurityStampAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyRevocationAttempted(admin);
        VerifyRefreshTokenCookieDeleted();
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(It.IsAny<AdminUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AccessTokenClaimAdminNotFound_ReturnsOkAndDeletesCookie()
    {
        SetupAccessTokenUser(AdminEmail);
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync((AdminUser?)null);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
    }

    [Fact]
    public async Task Handle_NoClaimAndValidRefreshCookie_RevokesSessionsAndDeletesCookie()
    {
        var admin = CreateAdmin(id: 42);
        SetupCookieSession(RawRefreshToken, admin, CreateSession(admin));
        _mockUserManager.Setup(x => x.UpdateSecurityStampAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyRevocationAttempted(admin);
        VerifyRefreshTokenCookieDeleted();
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(It.IsAny<AdminUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NoClaimAndSessionNotFound_ReturnsOkWithoutRevokingAndDeletesCookie()
    {
        var admin = CreateAdmin();
        SetupCookieSession(RawRefreshToken, admin, session: null);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(HashedRefreshToken, admin.RefreshToken);
        Assert.NotNull(admin.RefreshTokenValidTo);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_NoClaimAndMissingOrBlankCookie_ReturnsOkAndDeletesCookie(string? cookieValue)
    {
        SetupRefreshCookie(cookieValue);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken(It.IsAny<string>()), Times.Never);
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoClaimAndInvalidCookieToken_ReturnsOkWithoutRevokingAndDeletesCookie()
    {
        SetupRefreshCookie(RawRefreshToken);
        _mockTokenService
            .Setup(x => x.GetClaimsFromExpiredToken(RawRefreshToken))
            .Returns(Result.Fail<ClaimsPrincipal>(AuthConstants.InvalidToken));

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoClaimAndCookieWithoutEmailClaim_ReturnsOkWithoutRevokingAndDeletesCookie()
    {
        SetupRefreshCookie(RawRefreshToken);
        _mockTokenService
            .Setup(x => x.GetClaimsFromExpiredToken(RawRefreshToken))
            .Returns(Result.Ok(CreatePrincipal()));

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoClaimAndCookieAdminNotFound_ReturnsOkWithoutRevokingAndDeletesCookie()
    {
        SetupRefreshCookie(RawRefreshToken);
        _mockTokenService
            .Setup(x => x.GetClaimsFromExpiredToken(RawRefreshToken))
            .Returns(Result.Ok(CreatePrincipal(AdminEmail)));
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync((AdminUser?)null);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyNoRevocation();
        VerifyRefreshTokenCookieDeleted();
        _mockTokenService.Verify(
            x => x.VerifyRefreshTokenHash(It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateSecurityStampFails_ReloadsSavesRevocationsReturnsNotUpdatedAndDeletesCookie()
    {
        var admin = CreateAdmin();
        SetupAccessTokenUser(AdminEmail);
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync(admin);
        _mockUserManager
            .Setup(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = AuthConstants.NotUpdated }));

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.NotUpdated, result.Errors[0].Message);
        VerifyRevocationAttempted(admin, times: 1);
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        VerifyRefreshTokenCookieDeleted();
    }

    [Fact]
    public async Task Handle_GivenConcurrencyFailureOnUpdateSecurityStamp_RetriesWithFreshAdminAndSucceeds()
    {
        var admin = CreateAdmin();
        SetupAccessTokenUser(AdminEmail);
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync(admin);
        _mockUserManager
            .SetupSequence(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(CreateConcurrencyError()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        AssertSuccess(result);
        VerifyRevocationAttempted(admin, times: 2);
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
        VerifyRefreshTokenCookieDeleted();
    }

    [Fact]
    public async Task Handle_GivenConcurrencyFailureOnBothAttempts_ReloadsAndPersistsRevocationsAndReturnsNotUpdated()
    {
        var admin = CreateAdmin();
        SetupAccessTokenUser(AdminEmail);
        _mockUserManager.Setup(x => x.FindByEmailAsync(AdminEmail)).ReturnsAsync(admin);
        _mockUserManager
            .SetupSequence(x => x.UpdateSecurityStampAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(CreateConcurrencyError()))
            .ReturnsAsync(IdentityResult.Failed(CreateConcurrencyError()));

        var result = await _commandHandler.Handle(new LogoutCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.NotUpdated, result.Errors[0].Message);
        VerifyRevocationAttempted(admin, times: 2);
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(admin, It.IsAny<CancellationToken>()), Times.Exactly(2));
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
        VerifyRefreshTokenCookieDeleted();
    }

    private static Mock<UserManager<AdminUser>> CreateUserManagerMock() =>
        new(
            new Mock<IUserStore<AdminUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<AdminUser>>().Object,
            Array.Empty<IUserValidator<AdminUser>>(),
            Array.Empty<IPasswordValidator<AdminUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<AdminUser>>>().Object);

    private static AdminUser CreateAdmin(int id = 1, string refreshToken = HashedRefreshToken) =>
        new()
        {
            Id = id,
            Email = AdminEmail,
            RefreshToken = refreshToken,
            RefreshTokenValidTo = DateTimeOffset.UtcNow.AddDays(1)
        };

    private static ClaimsPrincipal CreatePrincipal(string? email = null)
    {
        Claim[] claims = email is null ? [] : [new Claim(ClaimTypes.Email, email)];
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    private static IdentityError CreateConcurrencyError() =>
        new()
        {
            Code = new IdentityErrorDescriber().ConcurrencyFailure().Code,
            Description = "Optimistic concurrency failure"
        };

    private void SetupAccessTokenUser(string email) =>
        _mockHttpContext.SetupGet(c => c.User).Returns(CreatePrincipal(email));

    private void SetupRefreshCookie(string? cookieValue)
    {
        string? outValue = cookieValue;
        _mockRequestCookies
            .Setup(c => c.TryGetValue(AuthConstants.RefreshTokenCookieName, out outValue))
            .Returns(cookieValue is not null);
    }

    private static RefreshTokenSession CreateSession(
        AdminUser admin,
        bool isRevoked = false,
        string? revocationReason = null,
        DateTimeOffset? revokedAt = null,
        DateTimeOffset? expiresAt = null,
        int? adminUserId = null) =>
        new()
    {
        AdminUserId = adminUserId ?? admin.Id,
        TokenHash = HashedRefreshToken,
        IsRevoked = isRevoked,
        RevocationReason = revocationReason,
        RevokedAt = revokedAt,
        ExpiresAt = expiresAt ?? DateTimeOffset.UtcNow.AddDays(1),
        CreatedAt = DateTimeOffset.UtcNow
    };

    private void SetupCookieSession(string rawToken, AdminUser admin, RefreshTokenSession? session)
    {
        SetupRefreshCookie(rawToken);
        _mockTokenService
            .Setup(x => x.GetClaimsFromExpiredToken(rawToken))
            .Returns(Result.Ok(CreatePrincipal(admin.Email)));
        _mockUserManager.Setup(x => x.FindByEmailAsync(admin.Email!)).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.HashRefreshToken(rawToken)).Returns(HashedRefreshToken);
        _mockRefreshTokenSessionRepository
            .Setup(x => x.GetByTokenHashAsync(HashedRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
    }

    private static void AssertSuccess(Result<Unit> result)
    {
        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);
    }

    private void VerifyRevocationAttempted(AdminUser admin, int times = 1)
    {
        Assert.Null(admin.RefreshToken);
        Assert.Null(admin.RefreshTokenValidTo);
        _mockRefreshTokenSessionRepository.Verify(
            x => x.RevokeAdminSessionsAsync(
                admin.Id,
                AuthConstants.RevocationReasons.Logout,
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(times));
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(admin), Times.Exactly(times));
    }

    private void VerifyNoRevocation()
    {
        _mockRefreshTokenSessionRepository.Verify(
            x => x.RevokeAdminSessionsAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _mockUserManager.Verify(x => x.UpdateSecurityStampAsync(It.IsAny<AdminUser>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.ReloadAsync(It.IsAny<AdminUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    private void VerifyRefreshTokenCookieDeleted()
    {
        _mockResponseCookies.Verify(
            c => c.Delete(
                AuthConstants.RefreshTokenCookieName,
                It.Is<CookieOptions>(options =>
                    options.HttpOnly
                    && options.Secure
                    && options.SameSite == SameSiteMode.Strict
                    && options.Path == AuthConstants.RefreshTokenCookiePath)),
            Times.Once);
    }
}
