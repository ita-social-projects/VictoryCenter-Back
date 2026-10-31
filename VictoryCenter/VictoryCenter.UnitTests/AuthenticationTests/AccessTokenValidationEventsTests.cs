using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Authentication;

namespace VictoryCenter.UnitTests.AuthenticationTests;

public class AccessTokenValidationEventsTests
{
    private const string AdminEmail = "admin@gmail.com";
    private const string SecurityStamp = "security_stamp";
    private const string SecurityStampHash = "security_stamp_hash";

    private readonly Mock<UserManager<AdminUser>> _mockUserManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AccessTokenValidationEvents _events;

    public AccessTokenValidationEventsTests()
    {
        _mockUserManager = new Mock<UserManager<AdminUser>>(
            new Mock<IUserStore<AdminUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<AdminUser>>().Object,
            Array.Empty<IUserValidator<AdminUser>>(),
            Array.Empty<IPasswordValidator<AdminUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<AdminUser>>>().Object);
        _mockTokenService = new Mock<ITokenService>();
        _events = new AccessTokenValidationEvents(_mockUserManager.Object, _mockTokenService.Object);
    }

    [Fact]
    public async Task TokenValidated_GivenCurrentSecurityStamp_DoesNotFail()
    {
        SetupAdmins(CreateAdmin(SecurityStamp));
        _mockTokenService.Setup(x => x.VerifySecurityStampHash(SecurityStamp, SecurityStampHash)).Returns(true);
        var context = CreateContext(
            new Claim(ClaimTypes.Email, AdminEmail),
            new Claim(AuthConstants.SecurityStampHashClaimType, SecurityStampHash));

        await _events.TokenValidated(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public async Task TokenValidated_GivenRotatedSecurityStamp_FailsAsRevoked()
    {
        SetupAdmins(CreateAdmin("rotated_security_stamp"));
        _mockTokenService.Setup(x => x.VerifySecurityStampHash("rotated_security_stamp", SecurityStampHash)).Returns(false);
        var context = CreateContext(
            new Claim(ClaimTypes.Email, AdminEmail),
            new Claim(AuthConstants.SecurityStampHashClaimType, SecurityStampHash));

        await _events.TokenValidated(context);

        Assert.Equal(AuthConstants.AccessTokenRevoked, context.Result?.Failure?.Message);
    }

    [Fact]
    public async Task TokenValidated_GivenUnknownAdmin_FailsAsRevoked()
    {
        SetupAdmins();
        var context = CreateContext(
            new Claim(ClaimTypes.Email, AdminEmail),
            new Claim(AuthConstants.SecurityStampHashClaimType, SecurityStampHash));

        await _events.TokenValidated(context);

        Assert.Equal(AuthConstants.AccessTokenRevoked, context.Result?.Failure?.Message);
        _mockTokenService.Verify(x => x.VerifySecurityStampHash(null, SecurityStampHash), Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task TokenValidated_GivenMissingRequiredClaim_FailsWithoutLookup(bool includeEmail, bool includeSecurityStampHash)
    {
        var claims = new List<Claim>();
        if (includeEmail)
        {
            claims.Add(new Claim(ClaimTypes.Email, AdminEmail));
        }

        if (includeSecurityStampHash)
        {
            claims.Add(new Claim(AuthConstants.SecurityStampHashClaimType, SecurityStampHash));
        }

        var context = CreateContext(claims.ToArray());

        await _events.TokenValidated(context);

        Assert.Equal(AuthConstants.InvalidToken, context.Result?.Failure?.Message);
        _mockUserManager.Verify(x => x.Users, Times.Never);
    }

    private static AdminUser CreateAdmin(string securityStamp)
    {
        return new AdminUser
        {
            Email = AdminEmail,
            NormalizedEmail = AdminEmail.ToUpperInvariant(),
            SecurityStamp = securityStamp
        };
    }

    private void SetupAdmins(params AdminUser[] admins)
    {
        _mockUserManager.Setup(x => x.NormalizeEmail(It.IsAny<string>())).Returns((string email) => email.ToUpperInvariant());
        _mockUserManager.Setup(x => x.Users).Returns(admins.BuildMock());
    }

    private static TokenValidatedContext CreateContext(params Claim[] claims)
    {
        var scheme = new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(JwtBearerHandler));
        return new TokenValidatedContext(new DefaultHttpContext(), scheme, new JwtBearerOptions())
        {
            Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme))
        };
    }
}
