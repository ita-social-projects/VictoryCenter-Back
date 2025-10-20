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

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Auth;

public class LogoutTests
{
    private readonly LogoutCommandHandler _commandHandler;
    private readonly Mock<UserManager<AdminUser>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

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
        _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<AdminUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateFails_ReturnsNotUpdated()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser();
        var mockHttpContext = new Mock<HttpContext>();
        var claims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")]));
        mockHttpContext.SetupGet(c => c.User).Returns(claims);
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
        _mockUserManager.Setup(x => x.FindByEmailAsync("admin@gmail.com")).ReturnsAsync(admin);
        _mockUserManager.Setup(x => x.UpdateAsync(admin))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = AuthConstants.NotUpdated }));

        var result = await _commandHandler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(AuthConstants.NotUpdated, result.Errors[0].Message);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
        Assert.Null(admin.RefreshToken);
        Assert.Equal(admin.RefreshTokenValidTo, DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task Handle_ValidData_SucceedsAndClearsCookies()
    {
        var cmd = new LogoutCommand();
        var admin = new AdminUser { RefreshToken = "refreshToken", RefreshTokenValidTo = DateTimeOffset.UtcNow.AddDays(1) };
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
        Assert.Equal(admin.RefreshTokenValidTo, DateTimeOffset.MinValue);
        _mockUserManager.Verify(x => x.FindByEmailAsync("admin@gmail.com"), Times.Once);
        _mockUserManager.Verify(x => x.UpdateAsync(admin), Times.Once);
        mockResponseCookies.Verify(
            c => c.Delete(It.Is<string>(s => s == AuthConstants.RefreshTokenCookieName)),
            Times.Once);
    }
}
