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
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ITokenService _tokenService;
    private readonly TimeProvider _timeProvider;

    public LogoutCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AdminUser> userManager,
        IRepositoryWrapper repositoryWrapper,
        ITokenService tokenService,
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
        var email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
        {
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var admin = await _userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            return Result.Fail(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        var now = _timeProvider.GetUtcNow();

        if (_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken))
        {
            var tokenHash = _tokenService.HashRefreshToken(refreshToken);
            var session = await _repositoryWrapper.RefreshTokenSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
            if (session != null)
            {
                await _repositoryWrapper.RefreshTokenSessionRepository.RevokeFamilyAsync(
                    session.FamilyId,
                    AuthConstants.RevocationReasons.Logout,
                    now,
                    cancellationToken);
            }
        }

        await _repositoryWrapper.RefreshTokenSessionRepository.RevokeAdminSessionsAsync(
            admin.Id,
            AuthConstants.RevocationReasons.Logout,
            now,
            cancellationToken);

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = null;

        var updateAdmin = await _userManager.UpdateAsync(admin);
        if (!updateAdmin.Succeeded)
        {
            return Result.Fail(AuthConstants.NotUpdated);
        }

        await _repositoryWrapper.SaveChangesAsync();

        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(
            AuthConstants.RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = AuthConstants.RefreshTokenCookiePath
            });

        return Result.Ok(Unit.Value);
    }
}
