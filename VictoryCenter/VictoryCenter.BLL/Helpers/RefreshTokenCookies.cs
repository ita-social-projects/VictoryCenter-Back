using Microsoft.AspNetCore.Http;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Helpers;
public static class RefreshTokenCookies
{
    public static void Append(IResponseCookies cookies, string token, DateTimeOffset expires)
    {
        DeleteLegacy(cookies);
        cookies.Append(AuthConstants.RefreshTokenCookieName, token, Options(AuthConstants.RefreshTokenCookiePath, expires));
    }

    public static void Delete(IResponseCookies cookies)
    {
        cookies.Delete(AuthConstants.RefreshTokenCookieName, Options(AuthConstants.RefreshTokenCookiePath));
        DeleteLegacy(cookies);
    }

    private static void DeleteLegacy(IResponseCookies cookies) =>
        cookies.Delete(AuthConstants.RefreshTokenCookieName, Options(AuthConstants.LegacyRefreshTokenCookiePath));

    private static CookieOptions Options(string path, DateTimeOffset? expires = null) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = path,
        Expires = expires
    };
}
