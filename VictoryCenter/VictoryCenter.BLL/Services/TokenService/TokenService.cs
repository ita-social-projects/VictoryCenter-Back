using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace VictoryCenter.BLL.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtOptions> jwtOptions, IConfiguration configuration, ILogger<TokenService> logger)
    {
        _jwtOptions = jwtOptions;
        _configuration = configuration;
        _logger = logger;
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
            expires: issuedAt.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays)),
            notBefore: issuedAt,
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.RefreshTokenSecretKey)), SecurityAlgorithms.HmacSha256));

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail(AuthConstants.RefreshTokenCannotBeNullOrEmpty);
        }

        var tokenValidationParameters = AuthHelper.GetTokenValidationParameters(_configuration).Clone();
        tokenValidationParameters.ValidateLifetime = false;
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.RefreshTokenSecretKey));
        try
        {
            var principal = _jwtSecurityTokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            {
                return Result.Fail(AuthConstants.InvalidToken);
            }

            return principal;
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, "An error occured in the {ServiceName}: {ErrorMessage}", nameof(TokenService), e.Message);
            return Result.Fail(AuthConstants.InvalidToken);
        }
        catch (SecurityTokenException e)
        {
            _logger.LogError(e, "An error occured in the {ServiceName}: {ErrorMessage}", nameof(TokenService), e.Message);
            return Result.Fail(AuthConstants.InvalidTokenSignature);
        }
    }
}
