using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.TokenService;

namespace VictoryCenter.UnitTests.ServiceTests;

public class TokenServiceTest
{
    private readonly ITokenService _tokenService;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly JwtOptions _jwtOptions;
    private readonly Mock<ILogger<TokenService>> _mockLogger;

    public TokenServiceTest()
    {
        _mockLogger = new Mock<ILogger<TokenService>>();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new("JwtOptions:Audience", "UnitTests.Client"),
                new("JwtOptions:Issuer", "UnitTests.Tested"),
                new("JwtOptions:LifetimeInMinutes", "15"),
                new("JwtOptions:Secretkey", "09DF83C7-1862-4AC2-B400-7FDA46861AC2")
            ])
            .Build();

        var jwtOptions = new JwtOptions()
        {
            Audience = "UnitTests.Client",
            Issuer = "UnitTests.Tested",
            LifetimeInMinutes = 15,
            SecretKey = "09DF83C7-1862-4AC2-B400-7FDA46861AC2",
            RefreshTokenSecretKey = "09DF83C7-1862-4AC2-B400-7FDA46861AC2",
            RefreshTokenLifetimeInDays = 7
        };

        var mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);
        IOptions<JwtOptions> jwtOptions1 = mockJwtOptions.Object;
        _jwtOptions = jwtOptions;
        _tokenService = new TokenService(jwtOptions1, configuration, _mockLogger.Object);
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

    [Fact]
    public void CreateAccessToken_GivenClaims_CreatesValidToken()
    {
        var token = _tokenService.CreateAccessToken([
            new Claim(JwtRegisteredClaimNames.Email, "admintest@gmail.com")
        ]);

        Assert.NotEmpty(token);

        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);

        var issuer = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss);
        var audience = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud);
        var jti = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
        var exp = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
        var nbf = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Nbf);
        var iat = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat);
        var email = jwtToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Email);

        Assert.True(issuer != null && issuer.Value == _jwtOptions.Issuer);
        Assert.True(audience != null && audience.Value == _jwtOptions.Audience);
        Assert.True(jti != null && !string.IsNullOrWhiteSpace(jti.Value));
        Assert.True(exp != null && !string.IsNullOrWhiteSpace(exp.Value));
        Assert.True(nbf != null && !string.IsNullOrWhiteSpace(nbf.Value));
        Assert.True(iat != null && !string.IsNullOrWhiteSpace(iat.Value));
        Assert.True(email is { Value: "admintest@gmail.com" });
    }

    [Fact]
    public void CreateAccessToken_GivenClaims_ExpiresAfterConfiguredLifetime()
    {
        var token = _tokenService.CreateAccessToken([]);

        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);

        Assert.Equal(TimeSpan.FromMinutes(_jwtOptions.LifetimeInMinutes), jwtToken.ValidTo - jwtToken.ValidFrom);
    }

    [Fact]
    public void CreateRefreshToken_SuccessfullyCreatesRefreshToken()
    {
        var refreshToken = _tokenService.CreateRefreshToken([]);

        Assert.NotEmpty(refreshToken);
    }

    [Fact]
    public void CreateRefreshToken_CalledTwice_CreatesUniqueTokens()
    {
        var firstRefreshToken = _tokenService.CreateRefreshToken([]);
        var secondRefreshToken = _tokenService.CreateRefreshToken([]);

        Assert.NotEqual(firstRefreshToken, secondRefreshToken);
    }

    [Fact]
    public void HashRefreshToken_GivenToken_ReturnsSha256Hash()
    {
        const string refreshToken = "refresh_token";

        var hash = _tokenService.HashRefreshToken(refreshToken);

        Assert.Equal(64, hash.Length);
        Assert.NotEqual(refreshToken, hash);
        Assert.True(_tokenService.VerifyRefreshTokenHash(refreshToken, hash));
    }

    [Theory]
    [InlineData("different_token")]
    [InlineData("")]
    [InlineData(null)]
    public void VerifyRefreshTokenHash_GivenNonMatchingOrMissingHash_ReturnsFalse(string? storedHash)
    {
        var isValid = _tokenService.VerifyRefreshTokenHash("refresh_token", storedHash);

        Assert.False(isValid);
    }

    [Fact]
    public void VerifyRefreshTokenHash_GivenLegacyPlaintextToken_ReturnsFalse()
    {
        var isValid = _tokenService.VerifyRefreshTokenHash("refresh_token", "refresh_token");

        Assert.False(isValid);
    }

    [Fact]
    public void HashSecurityStamp_GivenStamp_ReturnsVerifiableSha256Hash()
    {
        const string securityStamp = "SECURITYSTAMPVALUE";

        var hash = _tokenService.HashSecurityStamp(securityStamp);

        Assert.Equal(64, hash.Length);
        Assert.DoesNotContain(securityStamp, hash);
        Assert.True(_tokenService.VerifySecurityStampHash(securityStamp, hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void HashSecurityStamp_GivenEmptyStamp_ThrowsArgumentException(string securityStamp)
    {
        Assert.Throws<ArgumentException>(() => _tokenService.HashSecurityStamp(securityStamp));
    }

    [Fact]
    public void VerifySecurityStampHash_GivenRotatedStamp_ReturnsFalse()
    {
        var hashInToken = _tokenService.HashSecurityStamp("SECURITYSTAMPVALUE");

        var isValid = _tokenService.VerifySecurityStampHash("ROTATEDSTAMP", hashInToken);

        Assert.False(isValid);
    }

    [Theory]
    [InlineData(null, "ABCDEF")]
    [InlineData("SECURITYSTAMPVALUE", null)]
    [InlineData("SECURITYSTAMPVALUE", "")]
    [InlineData("SECURITYSTAMPVALUE", "SECURITYSTAMPVALUE")]
    public void VerifySecurityStampHash_GivenMissingOrMalformedValues_ReturnsFalse(string? currentStamp, string? hashInToken)
    {
        var isValid = _tokenService.VerifySecurityStampHash(currentStamp, hashInToken);

        Assert.False(isValid);
    }

    [Theory]
    [InlineData(0, 7, false)]
    [InlineData(61, 7, false)]
    [InlineData(1440, 7, false)]
    [InlineData(15, 0, false)]
    [InlineData(15, 31, false)]
    [InlineData(15, 7, true)]
    public void JwtOptions_ValidatesLifetimeRanges(int lifetimeInMinutes, int refreshTokenLifetimeInDays, bool expectedIsValid)
    {
        var options = new JwtOptions
        {
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            LifetimeInMinutes = lifetimeInMinutes,
            SecretKey = _jwtOptions.SecretKey,
            RefreshTokenSecretKey = _jwtOptions.RefreshTokenSecretKey,
            RefreshTokenLifetimeInDays = refreshTokenLifetimeInDays
        };

        var isValid = Validator.TryValidateObject(options, new ValidationContext(options), null, validateAllProperties: true);

        Assert.Equal(expectedIsValid, isValid);
    }

    [Fact]
    public void GetClaimsFromExpiredToken_GivenExpiredAccessToken_ReturnsPrincipalWithClaims()
    {
        var expiredToken = CreateExpiredToken();

        var principalResult = _tokenService.GetClaimsFromExpiredToken(expiredToken);

        var nameClaim = principalResult.Value.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
        Assert.NotNull(principalResult);
        Assert.True(nameClaim is { Value: "TestUser" });
    }

    [Fact]
    public void GetClaimsFromExpiredToken_GivenInvalidToken_ThrowsAnInvalidOperationException()
    {
        var invalidToken = "invalid token";

        var principalResult = _tokenService.GetClaimsFromExpiredToken(invalidToken);

        Assert.Equal("Invalid Token", principalResult.Errors[0].Message);
    }

    [Fact]
    public void GetClaimsFromExpiredToken_GivenEmptyToken_ThrowsAnInvalidOperationException()
    {
        var invalidToken = "";

        var principalResult = _tokenService.GetClaimsFromExpiredToken(invalidToken);

        Assert.Equal("Refresh token cannot be null or empty", principalResult.Errors[0].Message);
    }

    [Fact]
    public void GetClaimsFromExpiredToken_GivenTokenWithWrongAlgorithm_ThrowsAnInvalidOperationException()
    {
        var rsa = RSA.Create();
        var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }),
            Expires = DateTimeOffset.UtcNow.AddMinutes(-10).UtcDateTime,
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-11).UtcDateTime
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var principalResult = _tokenService.GetClaimsFromExpiredToken(jwt);
        Assert.Equal("Invalid Token signature", principalResult.Errors[0].Message);
    }

    private string CreateExpiredToken()
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "123")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTimeOffset.UtcNow.AddMinutes(-10).UtcDateTime,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-11).UtcDateTime,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
