using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AdminUser> _userManager;
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AdminUser> userManager,
        ITokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext!;

        DeleteRefreshTokenCookie(httpContext);

        var admin = await ResolveAdminAsync(httpContext);
        if (admin is null)
        {
            return Result.Ok(Unit.Value);
        }

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = null;

        // Rotating the stamp revokes every access token issued before logout; the call also persists the cleared refresh token.
        var updateAdmin = await _userManager.UpdateSecurityStampAsync(admin);
        return updateAdmin.Succeeded
            ? Result.Ok(Unit.Value)
            : Result.Fail(AuthConstants.NotUpdated);
    }

    private async Task<AdminUser?> ResolveAdminAsync(HttpContext httpContext)
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
        if (email is not null)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        if (!httpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        var cookieEmail = principalResult.IsSuccess
            ? principalResult.Value.FindFirstValue(ClaimTypes.Email)
            : null;
        if (cookieEmail is null)
        {
            return null;
        }

        var admin = await _userManager.FindByEmailAsync(cookieEmail);

        return admin is not null && _tokenService.VerifyRefreshTokenHash(refreshToken, admin.RefreshToken)
            ? admin
            : null;
    }

    private static void DeleteRefreshTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(
            AuthConstants.RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = AuthConstants.RefreshTokenCookiePath
            });
    }
}
