using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class RefreshTokenTests
{
    private static readonly DateTimeOffset FixedTestTime = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private readonly RefreshTokenCommandHandler _handler;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<AdminUser>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public RefreshTokenTests()
    {
        _mockTokenService = new Mock<ITokenService>();
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
        var jwtOptions = new JwtOptions
        {
            Audience = "UnitTests.Client",
            Issuer = "UnitTests.Tested",
            LifetimeInMinutes = 1440,
            SecretKey = "09DF83C7-1862-4AC2-B400-7FDA46861AC2",
            RefreshTokenSecretKey = "09DF83C7-1862-4AC2-B400-7FDA46861AC2",
            RefreshTokenLifetimeInDays = 7
        };

        var mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);
        IOptions<JwtOptions> jwtOptions1 = mockJwtOptions.Object;
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(x => x.GetUtcNow()).Returns(FixedTestTime);

        _handler = new RefreshTokenCommandHandler(
            _mockTokenService.Object,
            _mockUserManager.Object,
            _mockHttpContextAccessor.Object,
            jwtOptions1,
            timeProvider.Object);
    }

    [Fact]
    public async Task Handle_GivenEmptyExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out It.Ref<string>.IsAny!)).Returns(false);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenEmptyRefreshToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string empty = string.Empty;
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out empty!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenExpiredAccessTokenWithoutEmail_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(new ClaimsPrincipal());

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(Result.Fail("Invalid Token"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenTokenWithEmailOfNonExistingAdmin_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync((AdminUser?)null);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenRefreshTokenDifferentFromTheAdmins_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new AdminUser
        {
            RefreshToken = "refresh_token_different",
            RefreshTokenValidTo = FixedTestTime.AddHours(24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenOutdatedRefreshToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new AdminUser
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = FixedTestTime.AddHours(-24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidRefreshToken_ReturnsSuccess()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new AdminUser
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = FixedTestTime.AddHours(24),
            Email = "test@email.com"
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string cookieValue = "refresh_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out cookieValue!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("refresh_token")).Returns(claimsPrincipal);
        _mockTokenService.Setup(x => x.VerifyRefreshTokenHash("refresh_token", "refresh_token")).Returns(true);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken(It.IsAny<Claim[]>())).Returns("new_refresh_token");
        _mockTokenService.Setup(x => x.HashRefreshToken("new_refresh_token")).Returns("new_refresh_token_hash");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new_access_token", result.Value.AccessToken);
        Assert.Equal("new_refresh_token_hash", admin.RefreshToken);
        Assert.True(admin.RefreshTokenValidTo > FixedTestTime);
        mockResponseCookies.Verify(
            c => c.Append(
            It.Is<string>(s => s == "refreshToken"),
            It.Is<string>(s => s == "new_refresh_token"),
            It.IsAny<CookieOptions>()), Times.Once);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("refresh_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.HashRefreshToken("new_refresh_token"), Times.Once);
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenConcurrentRefresh_ReturnsUnauthorizedWithoutIssuingCookie()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new AdminUser
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = FixedTestTime.AddHours(24),
            Email = "test@email.com"
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string cookieValue = "refresh_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out cookieValue!)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("refresh_token")).Returns(claimsPrincipal);
        _mockTokenService.Setup(x => x.VerifyRefreshTokenHash("refresh_token", "refresh_token")).Returns(true);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken(It.IsAny<Claim[]>())).Returns("new_refresh_token");
        _mockTokenService.Setup(x => x.HashRefreshToken("new_refresh_token")).Returns("new_refresh_token_hash");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Failed(
            new IdentityError
            {
                Code = new IdentityErrorDescriber().ConcurrencyFailure().Code,
                Description = "Concurrency failure"
            }));
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.Unauthorized, result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("refresh_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.HashRefreshToken("new_refresh_token"), Times.Once);
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
        mockResponseCookies.Verify(
            c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()),
            Times.Never);
        mockResponseCookies.Verify(
            c => c.Delete(
                AuthConstants.RefreshTokenCookieName,
                It.Is<CookieOptions>(options => options.Path == AuthConstants.RefreshTokenCookiePath)),
            Times.Once);
    }
}

internal delegate void TryGetValueCallback(string key, out string value);
