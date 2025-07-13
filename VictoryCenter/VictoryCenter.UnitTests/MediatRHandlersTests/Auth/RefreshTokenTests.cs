using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Auth.RefreshToken;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class RefreshTokenTests
{
    private static readonly DateTime FixedTestTime = DateTime.UtcNow;
    private readonly RefreshTokenCommandHandler _handler;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<Admin>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockUserManager = new Mock<UserManager<Admin>>(
            new Mock<IUserStore<Admin>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Admin>>().Object,
            new IUserValidator<Admin>[0],
            new IPasswordValidator<Admin>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Admin>>>().Object);
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var jwtOptions = new JwtOptions()
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

        _jwtOptions = jwtOptions;
        _handler = new RefreshTokenCommandHandler(_mockTokenService.Object, _mockUserManager.Object, _mockHttpContextAccessor.Object, jwtOptions1);
    }

    [Fact]
    public async Task Handle_GivenEmptyExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out It.Ref<string>.IsAny)).Returns(false);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Refresh token is not present", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenEmptyRefreshToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string empty = string.Empty;
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out empty)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Refresh token is not present", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenExpiredAccessTokenWithoutEmail_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(new ClaimsPrincipal());

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid token", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(Result.Fail("Invalid Token"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid Token", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenTokenWithEmailOfNonExistingAdmin_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync((Admin?)null);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Admin with given email was not found", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenRefreshTokenDifferentFromTheAdmins_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new Admin()
        {
            RefreshToken = "refresh_token_different",
            RefreshTokenValidTo = FixedTestTime.AddHours(24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Refresh token is invalid", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenOutdatedRefreshToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new Admin()
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = FixedTestTime.AddHours(-24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        string token = "expired_access_token";
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out token)).Returns(true);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.SetupGet(r => r.Cookies).Returns(mockRequestCookies.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Refresh token is invalid", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidRefreshToken_ReturnsSuccess()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new Admin()
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
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out cookieValue)).Returns(true);
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
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken(It.IsAny<Claim[]>())).Returns("new_refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new_access_token", result.Value.AccessToken);
        Assert.Equal("new_refresh_token", admin.RefreshToken);
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
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidRefreshToken_ReturnsFailure()
    {
        var cmd = new RefreshTokenCommand();
        var admin = new Admin()
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
        mockRequestCookies.Setup(c => c.TryGetValue("refreshToken", out cookieValue)).Returns(true);
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
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken(It.IsAny<Claim[]>())).Returns("new_refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Failed" }));
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("refresh_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(It.IsAny<Claim[]>()), Times.Once);
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
    }
}

internal delegate void TryGetValueCallback(string key, out string value);
