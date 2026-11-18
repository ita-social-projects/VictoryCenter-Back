using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.TokenService;

namespace VictoryCenter.UnitTests.AuthenticationTests;

public class JwtValidationTests
{
    private readonly JwtOptions _options = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        SecretKey = new string('a', 64),
        RefreshTokenSecretKey = new string('b', 64),
        LifetimeInMinutes = 15,
        RefreshTokenLifetimeInDays = 7,
    };

    [Fact]
    public void CreatedAccessToken_PassesSharedValidation()
    {
        var token = CreateService().CreateAccessToken([new Claim(ClaimTypes.Email, "admin@example.test")]);

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, AuthHelper.GetTokenValidationParameters(_options), out _);

        Assert.Equal("admin@example.test", principal.FindFirstValue(ClaimTypes.Email));
    }

    [Theory]
    [InlineData("signature")]
    [InlineData("issuer")]
    [InlineData("audience")]
    [InlineData("expired")]
    [InlineData("future")]
    [InlineData("algorithm")]
    [InlineData("unsigned")]
    [InlineData("no-expiration")]
    [InlineData("refresh-key")]
    public void AccessValidation_RejectsInvalidToken(string scenario)
    {
        var token = CreateToken(scenario, _options.SecretKey);

        Assert.ThrowsAny<SecurityTokenException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, AuthHelper.GetTokenValidationParameters(_options), out _));
    }

    [Theory]
    [InlineData("signature")]
    [InlineData("issuer")]
    [InlineData("audience")]
    [InlineData("algorithm")]
    [InlineData("unsigned")]
    public void RefreshValidation_RejectsInvalidToken(string scenario)
    {
        Assert.True(CreateService().GetClaimsFromExpiredToken(CreateToken(scenario, _options.RefreshTokenSecretKey)).IsFailed);
    }

    [Fact]
    public void RefreshValidation_UsesRefreshKeyAndPreservesSeparateExpiryHandling()
    {
        var service = CreateService();
        var expiredRefresh = CreateToken("expired", _options.RefreshTokenSecretKey);

        Assert.True(service.GetClaimsFromExpiredToken(expiredRefresh).IsSuccess);
        Assert.True(service.GetClaimsFromExpiredToken(service.CreateAccessToken([])).IsFailed);
        Assert.True(service.GetClaimsFromExpiredToken(service.CreateRefreshToken([])).IsSuccess);
        Assert.True(AuthHelper.GetTokenValidationParameters(_options).ValidateLifetime);
    }

    [Theory]
    [InlineData(20, true)]
    [InlineData(90, false)]
    public void AccessValidation_PreservesThirtySecondClockSkew(int secondsExpired, bool expectedValid)
    {
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            notBefore: DateTime.UtcNow.AddMinutes(-5),
            expires: DateTime.UtcNow.AddSeconds(-secondsExpired),
            signingCredentials: Credentials(_options.SecretKey, SecurityAlgorithms.HmacSha256));
        var encoded = new JwtSecurityTokenHandler().WriteToken(token);
        var exception = Record.Exception(() => new JwtSecurityTokenHandler().ValidateToken(
            encoded, AuthHelper.GetTokenValidationParameters(_options), out _));

        Assert.Equal(expectedValid, exception is null);
    }

    private TokenService CreateService() => new(Options.Create(_options), NullLogger<TokenService>.Instance);

    private string CreateToken(string scenario, string key)
    {
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: scenario == "issuer" ? "other-issuer" : _options.Issuer,
            audience: scenario == "audience" ? "other-audience" : _options.Audience,
            notBefore: scenario == "future" ? now.AddMinutes(2) : now.AddMinutes(-5),
            expires: scenario == "no-expiration" ? null : scenario == "expired" ? now.AddMinutes(-2) : now.AddMinutes(5),
            signingCredentials: scenario == "unsigned" ? null : Credentials(
                scenario == "signature" ? new string('c', 64) : scenario == "refresh-key" ? _options.RefreshTokenSecretKey : key,
                scenario == "algorithm" ? SecurityAlgorithms.HmacSha384 : SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static SigningCredentials Credentials(string key, string algorithm) =>
        new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), algorithm);
}
