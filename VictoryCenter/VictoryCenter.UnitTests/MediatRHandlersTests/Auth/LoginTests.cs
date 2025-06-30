using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Auth.Login;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Validators.Auth;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class LoginTests
{
    private readonly LoginCommandHandler _commandHandler;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<Admin>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    public LoginTests()
    {
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
        _mockTokenService = new Mock<ITokenService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _commandHandler = new LoginCommandHandler(_mockTokenService.Object, _mockUserManager.Object, new LoginCommandValidator(), _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_GivenRequestWithInvalidEmail_ReturnsFail()
    {
        var cmd = new LoginCommand(new LoginRequest("invalid email", "Pa$$w0rd!"));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email address must be in a valid format", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenRequestWithEmptyEmail_ReturnsFail()
    {
        var cmd = new LoginCommand(new LoginRequest("", "Pa$$w0rd!"));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email cannot be empty", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenRequestWithEmptyPassword_ReturnsFail()
    {
        var cmd = new LoginCommand(new LoginRequest("admin@gmail.com", ""));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Password cannot be empty", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_AdminWithGivenEmailDoesNotExist_ReturnsFail()
    {
        var cmd = new LoginCommand(new LoginRequest("admin@gmail.com", "Pa$$w0rd!"));
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync((Admin?)null);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Admin with given email was not found", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenIncorrectPassword_ReturnsFail()
    {
        var cmd = new LoginCommand(new LoginRequest("admin@gmail.com", "Pa$$w0rd!"));
        var admin = new Admin();
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!")).ReturnsAsync(false);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Incorrect password", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!"), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidData_UpdatesSuccessfully()
    {
        var cmd = new LoginCommand(new LoginRequest("admin@gmail.com", "Pa$$w0rd!"));
        var admin = new Admin();

        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!")).ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("access_token", result.Value.AccessToken);
        Assert.Equal("refresh_token", result.Value.RefreshToken);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidData_UpdatesUnsuccessfully()
    {
        var cmd = new LoginCommand(new LoginRequest("admin@gmail.com", "Pa$$w0rd!"));
        var admin = new Admin();

        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!")).ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetClaimsAsync(admin)).ReturnsAsync([]);
        _mockTokenService.Setup(x => x.CreateAccessToken(It.IsAny<Claim[]>())).Returns("access_token");
        _mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("refresh_token");
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Failed" }));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.CheckPasswordAsync(admin, "Pa$$w0rd!"), Times.Once);
        _mockTokenService.Verify(x => x.CreateAccessToken(It.IsAny<Claim[]>()), Times.Once);
        _mockTokenService.Verify(x => x.CreateRefreshToken(), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
    }
}
