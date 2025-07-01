using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Options;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace VictoryCenter.BLL.Services;

public class TokenService : ITokenService
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly IConfiguration _configuration;

    public TokenService(IOptions<JwtOptions> jwtOptions, IConfiguration configuration)
    {
        _jwtOptions = jwtOptions;
        _configuration = configuration;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

    public string CreateAccessToken(Claim[] claims)
    {
        var issuedAt = DateTime.UtcNow;
        claims =
        [
            ..claims,
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Value.Issuer),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(issuedAt).ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var token = new JwtSecurityToken(
            audience: _jwtOptions.Value.Audience,
            issuer: _jwtOptions.Value.Issuer,
            expires: issuedAt.AddMinutes(_jwtOptions.Value.LifetimeInMinutes),
            notBefore: issuedAt,
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey)), SecurityAlgorithms.HmacSha256));

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(Claim[] claims)
    {
        var issuedAt = DateTime.UtcNow;
        claims =
        [
            ..claims,
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Value.Issuer),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(issuedAt).ToString(), ClaimValueTypes.Integer64),
        ];

        var token = new JwtSecurityToken(
            audience: _jwtOptions.Value.Audience,
            issuer: _jwtOptions.Value.Issuer,
            expires: issuedAt.Add(Constants.Authentication.RefreshTokenLifeTime),
            notBefore: issuedAt,
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.RefreshTokenSecretKey)), SecurityAlgorithms.HmacSha256));

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string refreshToken)
    {
        var tokenValidationParameters = Constants.Authentication.GetDefaultTokenValidationParameters(_configuration).Clone();
        tokenValidationParameters.ValidateLifetime = false;
        tokenValidationParameters.ValidateIssuerSigningKey = false;
        tokenValidationParameters.SignatureValidator = (token, parameters) => new JwtSecurityToken(token);
        try
        {
            var principal = _jwtSecurityTokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            {
                return Result.Fail("Invalid Token");
            }

            return principal;
        }
        catch (ArgumentException e)
        {
            return Result.Fail("Invalid token");
        }
    }
}
