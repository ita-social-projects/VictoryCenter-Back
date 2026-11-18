using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private static readonly string ConcurrencyFailureCode = new IdentityErrorDescriber().ConcurrencyFailure().Code;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AdminUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;

    public LogoutCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AdminUser> userManager,
        ITokenService tokenService,
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _tokenService = tokenService;
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext!;

        // Always clear the cookie, whatever the outcome below.
        DeleteRefreshTokenCookie(httpContext);

        var admin = await ResolveAdminAsync(httpContext);
        if (admin is null)
        {
            // Nothing identifiable to revoke (no session, stale or forged cookie). Logout stays idempotent.
            return Result.Ok(Unit.Value);
        }

        await RevokeAdminSessionStateAsync(admin, cancellationToken);

        // Rotating the stamp revokes every access token issued before logout;
        // the call also persists session revocations and the cleared refresh token.
        var updateAdmin = await _userManager.UpdateSecurityStampAsync(admin);
        if (!updateAdmin.Succeeded && updateAdmin.Errors.Any(e => e.Code == ConcurrencyFailureCode))
        {
            await _repositoryWrapper.ReloadAsync(admin, cancellationToken);
            await RevokeAdminSessionStateAsync(admin, cancellationToken);
            updateAdmin = await _userManager.UpdateSecurityStampAsync(admin);
        }

        if (!updateAdmin.Succeeded)
        {
            await _repositoryWrapper.ReloadAsync(admin, cancellationToken);
            await _repositoryWrapper.SaveChangesAsync();

            return Result.Fail(AuthConstants.NotUpdated);
        }

        return Result.Ok(Unit.Value);
    }

    private async Task RevokeAdminSessionStateAsync(AdminUser admin, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        await _repositoryWrapper.RefreshTokenSessionRepository.RevokeAdminSessionsAsync(
            admin.Id,
            AuthConstants.RevocationReasons.Logout,
            now,
            cancellationToken);

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = null;
    }

    private async Task<AdminUser?> ResolveAdminAsync(HttpContext httpContext)
    {
        // 1. A valid access token identifies the admin directly.
        var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
        if (email is not null)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // 2. Otherwise fall back to the refresh cookie, which works even after the access token has expired.
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

        // The cookie must match the stored hash, so a stale or forged token cannot revoke someone's session.
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