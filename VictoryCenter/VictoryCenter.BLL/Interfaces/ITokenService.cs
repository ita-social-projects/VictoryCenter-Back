using System.Security.Claims;
using FluentResults;

namespace VictoryCenter.BLL.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Claim[] claims);
    string CreateRefreshToken();
    Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string expiredAccessToken);
}
