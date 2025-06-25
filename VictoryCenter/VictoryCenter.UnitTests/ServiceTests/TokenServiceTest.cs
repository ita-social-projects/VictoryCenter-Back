using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services;

namespace VictoryCenter.UnitTests.ServiceTests;

public class TokenServiceTest
{
    private readonly ITokenService _tokenService;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly JwtOptions _jwtOptions;

    public TokenServiceTest()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new("JwtOptions:Audience", "UnitTests.Client"),
                new("JwtOptions:Issuer", "UnitTests.Tested"),
                new("JwtOptions:LifetimeInMinutes", "1440"),
                new("JwtOptions:Secretkey", "09DF83C7-1862-4AC2-B400-7FDA46861AC2")
            ])
            .Build();

        var jwtOptions = new JwtOptions()
        {
            Audience = "UnitTests.Client",
            Issuer = "UnitTests.Tested",
            LifetimeInMinutes = 1440,
            SecretKey = "09DF83C7-1862-4AC2-B400-7FDA46861AC2"
        };

        var mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);
        IOptions<JwtOptions> jwtOptions1 = mockJwtOptions.Object;

        _jwtOptions = jwtOptions;
        _tokenService = new TokenService(jwtOptions1, configuration);
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
    public void CreateRefreshToken_SuccessfullyCreatesRefreshToken()
    {
        var refreshToken = _tokenService.CreateRefreshToken();

        Assert.NotEmpty(refreshToken);
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

        Assert.Equal("Invalid token", principalResult.Errors[0].Message);
    }

    [Fact]
    public void GetClaimsFromExpiredToken_GivenTokenWithWrongAlgorithm_ThrowsAnInvalidOperationException()
    {
        var rsa = RSA.Create();
        var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }),
            Expires = DateTime.UtcNow.AddMinutes(-10),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            NotBefore = DateTime.UtcNow.AddMinutes(-11)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var principalResult = _tokenService.GetClaimsFromExpiredToken(jwt);
        Assert.Equal("Invalid Token", principalResult.Errors[0].Message);
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
            Expires = DateTime.UtcNow.AddMinutes(-10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            NotBefore = DateTime.UtcNow.AddMinutes(-11),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
