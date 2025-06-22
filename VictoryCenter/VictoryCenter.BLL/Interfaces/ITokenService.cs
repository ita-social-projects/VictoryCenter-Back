using System.Security.Claims;

namespace VictoryCenter.BLL.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Claim[] claims);
    string CreateRefreshToken();
    ClaimsPrincipal GetClaimsFromExpiredToken(string expiredAccessToken);
}
