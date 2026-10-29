using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private static readonly string ConcurrencyFailureCode = new IdentityErrorDescriber().ConcurrencyFailure().Code;

    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly TimeProvider _timeProvider;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        UserManager<AdminUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions,
        TimeProvider timeProvider,
        IRepositoryWrapper repositoryWrapper,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
        _timeProvider = timeProvider;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenRetrieved = _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken);
        if (!refreshTokenRetrieved || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        if (principalResult.IsFailed)
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var email = principalResult.Value.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var session = await _repositoryWrapper.RefreshTokenSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        var now = _timeProvider.GetUtcNow();

        if (session is null)
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        if (session.IsRevoked)
        {
            _logger.LogWarning(
                "Suspected refresh token reuse detected for AdminUserId: {AdminUserId}, FamilyId: {FamilyId}, Reason: {RevocationReason}",
                session.AdminUserId,
                session.FamilyId,
                session.RevocationReason);

            await _repositoryWrapper.RefreshTokenSessionRepository.RevokeFamilyAsync(
                session.FamilyId,
                "SuspectedTokenReuse",
                now,
                cancellationToken);

            admin.RefreshToken = null;
            admin.RefreshTokenValidTo = null;
            await _userManager.UpdateAsync(admin);
            await _repositoryWrapper.SaveChangesAsync();

            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        if (session.ExpiresAt <= now)
        {
            session.IsRevoked = true;
            session.RevokedAt = now;
            session.RevocationReason = "Expired";

            if (admin.RefreshToken == tokenHash)
            {
                admin.RefreshToken = null;
                admin.RefreshTokenValidTo = null;
                await _userManager.UpdateAsync(admin);
            }

            await _repositoryWrapper.SaveChangesAsync();

            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var familyId = session.FamilyId;
        var newRefreshToken = _tokenService.CreateRefreshToken([
            new Claim(ClaimTypes.Email, admin.Email!),
            new Claim("family_id", familyId.ToString())
        ]);
        var newTokenHash = _tokenService.HashRefreshToken(newRefreshToken);
        var newRefreshTokenExpires = now.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));

        session.IsRevoked = true;
        session.RevokedAt = now;
        session.RevocationReason = "Rotated";
        session.ReplacedByTokenHash = newTokenHash;

        var newSession = new RefreshTokenSession
        {
            AdminUserId = admin.Id,
            FamilyId = familyId,
            TokenHash = newTokenHash,
            ExpiresAt = newRefreshTokenExpires,
            CreatedAt = now,
            IsRevoked = false
        };

        await _repositoryWrapper.RefreshTokenSessionRepository.CreateAsync(newSession);

        admin.RefreshToken = newTokenHash;
        admin.RefreshTokenValidTo = newRefreshTokenExpires;

        var updateResult = await _userManager.UpdateAsync(admin);
        if (!updateResult.Succeeded)
        {
            DeleteRefreshTokenCookie();
            if (updateResult.Errors.Any(error => error.Code == ConcurrencyFailureCode))
            {
                return Result.Fail(AuthConstants.Unauthorized);
            }

            return Result.Fail(updateResult.Errors.Select(error => error.Description));
        }

        try
        {
            await _repositoryWrapper.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);

        AppendRefreshTokenCookie(newRefreshToken, newRefreshTokenExpires);
        return Result.Ok(new AuthResponseDto(accessToken));
    }

    private void AppendRefreshTokenCookie(string refreshToken, DateTimeOffset expires)
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response?.Cookies.Append(
            AuthConstants.RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = AuthConstants.RefreshTokenCookiePath
            });
    }

    private void DeleteRefreshTokenCookie()
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response?.Cookies.Delete(
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
