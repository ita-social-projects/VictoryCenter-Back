using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Auth.Logout;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using MediatR;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class LogoutTests
{
    private readonly LogoutCommandHandler _commandHandler;
    private readonly Mock<UserManager<Admin>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public LogoutTests()
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

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _commandHandler = new LogoutCommandHandler(_mockHttpContextAccessor.Object, _mockUserManager.Object);
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
        Assert.Equal("Unauthorized", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsNotFound()
    {
        var cmd = new LogoutCommand();
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "admin@gmail.com") }));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync((Admin?)null);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found admin", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<Admin>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateFails_ReturnsNotUpdated()
    {
        var cmd = new LogoutCommand();
        var admin = new Admin();
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "admin@gmail.com") }));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Not updated" }));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not updated", result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
        Assert.Null(admin.RefreshToken);
        Assert.Null(admin.RefreshTokenValidTo);
    }

    [Fact]
    public async Task Handle_ValidData_SucceedsAndClearsCookies()
    {
        var cmd = new LogoutCommand();
        var admin = new Admin { RefreshToken = "refresh_token", RefreshTokenValidTo = DateTime.UtcNow.AddDays(1) };
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "admin@gmail.com") }));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateAsync(admin)).ReturnsAsync(IdentityResult.Success);

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);
        Assert.Null(admin.RefreshToken);
        Assert.Null(admin.RefreshTokenValidTo);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
        mockResponseCookies.Verify(
            c => c.Delete(It.Is<string>(s => s == AuthConstants.RefreshTokenCookieName)),
            Times.Once);
    }
}
