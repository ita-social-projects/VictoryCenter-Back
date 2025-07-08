using System.Security.Claims;
using FluentResults;

namespace VictoryCenter.BLL.Interfaces.TokenService;

public interface ITokenService
{
    string CreateAccessToken(Claim[] claims);
    string CreateRefreshToken(Claim[] claims);
    Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string refreshToken);
}
