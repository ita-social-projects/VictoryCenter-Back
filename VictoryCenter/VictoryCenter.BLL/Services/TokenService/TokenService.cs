using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        var issuedAt = DateTimeOffset.UtcNow.UtcDateTime;
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
        var issuedAt = DateTimeOffset.UtcNow.UtcDateTime;
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
            expires: issuedAt.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays)),
            notBefore: issuedAt,
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.RefreshTokenSecretKey)), SecurityAlgorithms.HmacSha256));

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public string HashRefreshToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }

    public bool VerifyRefreshTokenHash(string refreshToken, string? storedHash)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        byte[] storedHashBytes;
        try
        {
            storedHashBytes = Convert.FromHexString(storedHash);
        }
        catch (FormatException)
        {
            // Existing plaintext refresh tokens are intentionally invalid after deployment.
            return false;
        }

        var providedHashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return CryptographicOperations.FixedTimeEquals(providedHashBytes, storedHashBytes);
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
            _logger.LogWarning(
                "Refresh token validation failed in {ServiceName} with {ExceptionType}",
                nameof(TokenService),
                e.GetType().Name);
            return Result.Fail(AuthConstants.InvalidToken);
        }
        catch (SecurityTokenException e)
        {
            _logger.LogWarning(
                "Refresh token validation failed in {ServiceName} with {ExceptionType}",
                nameof(TokenService),
                e.GetType().Name);
            return Result.Fail(AuthConstants.InvalidTokenSignature);
        }
    }
}
