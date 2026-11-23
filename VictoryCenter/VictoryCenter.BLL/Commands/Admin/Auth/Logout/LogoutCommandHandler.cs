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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AdminUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private static readonly string ConcurrencyFailureCode = new IdentityErrorDescriber().ConcurrencyFailure().Code;

    public LogoutCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AdminUser> userManager,
        ITokenService tokenService,
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _repositoryWrapper = repositoryWrapper;
        _tokenService = tokenService;
        _timeProvider = timeProvider;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext!;
        RefreshTokenCookies.Delete(httpContext.Response.Cookies);

        var admin = await ResolveAdminAsync(httpContext, cancellationToken);
        if (admin is null)
        {
            return Result.Ok(Unit.Value);
        }

        return await RevokeAsync(admin, cancellationToken);
    }

    private async Task<AdminUser?> ResolveAdminAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
        if (email is not null)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        if (!httpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var token)
            || string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var principal = _tokenService.GetClaimsFromExpiredToken(token);
        var cookieEmail = principal.IsSuccess ? principal.Value.FindFirstValue(ClaimTypes.Email) : null;
        if (cookieEmail is null)
        {
            return null;
        }

        var admin = await _userManager.FindByEmailAsync(cookieEmail);
        if (admin is null)
        {
            return null;
        }

        var tokenHash = _tokenService.HashRefreshToken(token);
        var session = await _repositoryWrapper.RefreshTokenSessionRepository.GetByTokenHashAsync(tokenHash);

        return session is not null
            && session.AdminUserId == admin.Id
            && IsAcceptable(session, _timeProvider.GetUtcNow())
                ? admin
                : null;
    }

    private static bool IsAcceptable(RefreshTokenSession session, DateTimeOffset now)
    {
        if (!session.IsRevoked)
        {
            return session.ExpiresAt > now;
        }

        return session.RevocationReason == AuthConstants.RevocationReasons.Rotated
            && session.RevokedAt is not null
            && now - session.RevokedAt.Value <= AuthConstants.RotationGracePeriod;
    }

    private async Task<Result<Unit>> RevokeAsync(AdminUser admin, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();
        await _repositoryWrapper.RefreshTokenSessionRepository.RevokeAdminSessionsAsync(
            admin.Id,
            AuthConstants.RevocationReasons.Logout,
            now);

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = null;

        var updateAdmin = await _userManager.UpdateSecurityStampAsync(admin);

        if (!updateAdmin.Succeeded && updateAdmin.Errors.Any(error => error.Code == ConcurrencyFailureCode))
        {
            await _repositoryWrapper.ReloadAsync(admin);

            var retryNow = _timeProvider.GetUtcNow();
            await _repositoryWrapper.RefreshTokenSessionRepository.RevokeAdminSessionsAsync(
                admin.Id,
                AuthConstants.RevocationReasons.Logout,
                retryNow);

            admin.RefreshToken = null;
            admin.RefreshTokenValidTo = null;
            updateAdmin = await _userManager.UpdateSecurityStampAsync(admin);
        }

        if (!updateAdmin.Succeeded)
        {
            await _repositoryWrapper.ReloadAsync(admin);
            await _repositoryWrapper.SaveChangesAsync();
            return Result.Fail(AuthConstants.NotUpdated);
        }

        return Result.Ok(Unit.Value);
    }
}
