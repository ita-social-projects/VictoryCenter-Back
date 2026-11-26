using System.Globalization;
using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.BLL.Helpers;

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
        var httpContext = _httpContextAccessor.HttpContext!;
        var refreshTokenRetrieved = httpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken);
        if (!refreshTokenRetrieved || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail(new UnauthorizedError(AuthConstants.Unauthorized));
        }

        var identity = await ResolveIdentityAsync(httpContext, refreshToken);
        if (identity is null)
        {
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var (admin, email) = identity.Value;

        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var session = await _repositoryWrapper.RefreshTokenSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        var now = _timeProvider.GetUtcNow();

        if (session is null || session.AdminUserId != admin.Id)
        {
            return FailAndDeleteCookie(httpContext);
        }

        if (session.IsRevoked)
        {
            return await HandleRevokedSessionAsync(httpContext, session, now, cancellationToken);
        }

        if (session.ExpiresAt <= now)
        {
            await ExpireSessionAsync(session, now);
            return FailAndDeleteCookie(httpContext);
        }

        return await RotateSessionAsync(httpContext, admin, email, session, now);
    }

    private async Task<(AdminUser Admin, Claim Email)?> ResolveIdentityAsync(HttpContext httpContext, string refreshToken)
    {
        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        if (principalResult.IsFailed)
        {
            RefreshTokenCookies.Delete(httpContext.Response.Cookies);
            return null;
        }

        var email = principalResult.Value.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            RefreshTokenCookies.Delete(httpContext.Response.Cookies);
            return null;
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            RefreshTokenCookies.Delete(httpContext.Response.Cookies);
            return null;
        }

        return (admin, email);
    }

    private async Task<Result<AuthResponseDto>> HandleRevokedSessionAsync(
        HttpContext httpContext,
        RefreshTokenSession session,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (IsRecentlyRotated(session, now))
        {
            _logger.LogInformation(
                "Refresh token was recently rotated within grace period for AdminUserId: {AdminUserId}, FamilyId: {FamilyId}",
                session.AdminUserId,
                session.FamilyId);

            return Result.Fail(AuthConstants.Unauthorized);
        }

        _logger.LogWarning(
            "Suspected refresh token reuse detected for AdminUserId: {AdminUserId}, FamilyId: {FamilyId}, Reason: {RevocationReason}",
            session.AdminUserId,
            session.FamilyId,
            session.RevocationReason);

        await _repositoryWrapper.RefreshTokenSessionRepository.RevokeFamilyAsync(
            session.FamilyId,
            AuthConstants.RevocationReasons.SuspectedTokenReuse,
            now,
            cancellationToken);

        await _repositoryWrapper.SaveChangesAsync();

        return FailAndDeleteCookie(httpContext);
    }

    private static bool IsRecentlyRotated(RefreshTokenSession session, DateTimeOffset now)
    {
        return session.RevocationReason == AuthConstants.RevocationReasons.Rotated
            && session.RevokedAt.HasValue
            && now - session.RevokedAt.Value <= AuthConstants.RotationGracePeriod;
    }

    private async Task ExpireSessionAsync(RefreshTokenSession session, DateTimeOffset now)
    {
        session.IsRevoked = true;
        session.RevokedAt = now;
        session.RevocationReason = AuthConstants.RevocationReasons.Expired;

        await _repositoryWrapper.SaveChangesAsync();
    }

    private async Task<Result<AuthResponseDto>> RotateSessionAsync(
        HttpContext httpContext,
        AdminUser admin,
        Claim email,
        RefreshTokenSession session,
        DateTimeOffset now)
    {
        var familyId = session.FamilyId;
        var newRefreshToken = _tokenService.CreateRefreshToken([
            new Claim(ClaimTypes.Email, admin.Email!),
            new Claim(AuthConstants.FamilyIdClaimType, familyId.ToString())
        ]);
        var newTokenHash = _tokenService.HashRefreshToken(newRefreshToken);
        var newRefreshTokenExpires = now.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));

        session.IsRevoked = true;
        session.RevokedAt = now;
        session.RevocationReason = AuthConstants.RevocationReasons.Rotated;
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
            return HandleUpdateFailure(httpContext, admin, session, updateResult);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email,
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(AuthConstants.SecurityStampHashClaimType, _tokenService.HashSecurityStamp(admin.SecurityStamp!))
        ]);
        RefreshTokenCookies.Append(httpContext.Response.Cookies, newRefreshToken, newRefreshTokenExpires);
        return Result.Ok(new AuthResponseDto(accessToken));
    }

    private Result<AuthResponseDto> HandleUpdateFailure(
        HttpContext httpContext,
        AdminUser admin,
        RefreshTokenSession session,
        IdentityResult updateResult)
    {
        if (updateResult.Errors.Any(error => error.Code == ConcurrencyFailureCode))
        {
            _logger.LogInformation(
                "Concurrent refresh collision detected when updating admin user for AdminUserId: {AdminUserId}, FamilyId: {FamilyId}",
                admin.Id,
                session.FamilyId);

            return Result.Fail(AuthConstants.Unauthorized);
        }

        _logger.LogError(
            "Failed to update admin user {AdminUserId} during refresh: {Errors}",
            admin.Id,
            string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        return FailAndDeleteCookie(httpContext);
    }

    private static Result<AuthResponseDto> FailAndDeleteCookie(HttpContext httpContext)
    {
        RefreshTokenCookies.Delete(httpContext.Response.Cookies);
        return Result.Fail(AuthConstants.Unauthorized);
    }
}
