using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Validators.Auth;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class RefreshTokenTests
{
    private readonly RefreshTokenCommandHandler _handler;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<Admin>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

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
        _handler = new RefreshTokenCommandHandler(_mockTokenService.Object, _mockUserManager.Object, new RefreshTokenCommandValidator(), _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_GivenEmptyExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("", "refresh_token"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Expired access token cannot be empty", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenEmptyRefreshToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", ""));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Refresh token cannot be empty", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenExpiredAccessTokenWithoutEmail_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(new ClaimsPrincipal());

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid token", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidExpiredAccessToken_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(Result.Fail("Invalid Token"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid Token", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenTokenWithEmailOfNonExistingAdmin_ReturnsFail()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));

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
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));
        var admin = new Admin()
        {
            RefreshToken = "refresh_token_different",
            RefreshTokenValidTo = DateTime.UtcNow.AddHours(24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

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
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));
        var admin = new Admin()
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = DateTime.UtcNow.AddHours(-24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

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
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));
        var admin = new Admin()
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = DateTime.UtcNow.AddHours(24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("new_refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new_access_token", result.Value.AccessToken);
        Assert.Equal("new_refresh_token", result.Value.RefreshToken);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(), Times.Once);
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidRefreshToken_ReturnsFailure()
    {
        var cmd = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));
        var admin = new Admin()
        {
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = DateTime.UtcNow.AddHours(24)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Email, "test@email.com")
        ]));

        _mockTokenService.Setup(x => x.GetClaimsFromExpiredToken("expired_access_token")).Returns(claimsPrincipal);
        _mockUserManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(admin);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("new_access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("new_refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Failed" }));
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed", result.Errors[0].Message);
        _mockTokenService.Verify(x => x.GetClaimsFromExpiredToken("expired_access_token"), Times.Once);
        _mockUserManager.Verify(x => x.FindByEmailAsync("test@email.com"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(), Times.Once);
        _mockUserManager.Verify(x => x.GetClaimsAsync(admin), Times.Once);
    }
}
