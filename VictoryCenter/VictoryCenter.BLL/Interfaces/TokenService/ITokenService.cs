using System.Security.Claims;
using FluentResults;

namespace VictoryCenter.BLL.Interfaces.TokenService;

public interface ITokenService
{
    string CreateAccessToken(Claim[] claims);
    string CreateRefreshToken(Claim[] claims);
    string HashRefreshToken(string refreshToken);
    bool VerifyRefreshTokenHash(string refreshToken, string? storedHash);
    Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string refreshToken);
}
