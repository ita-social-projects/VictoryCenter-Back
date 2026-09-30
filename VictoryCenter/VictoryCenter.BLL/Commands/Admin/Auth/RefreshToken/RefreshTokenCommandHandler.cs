using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private static readonly string ConcurrencyFailureCode = new IdentityErrorDescriber().ConcurrencyFailure().Code;

    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly TimeProvider _timeProvider;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        UserManager<AdminUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions,
        TimeProvider timeProvider)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
        _timeProvider = timeProvider;
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

        if (admin.RefreshTokenValidTo <= _timeProvider.GetUtcNow()
            || !_tokenService.VerifyRefreshTokenHash(refreshToken, admin.RefreshToken))
        {
            DeleteRefreshTokenCookie();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var newRefreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, admin.Email!)]);
        var refreshTokenExpires = _timeProvider.GetUtcNow().Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));
        admin.RefreshToken = _tokenService.HashRefreshToken(newRefreshToken);
        admin.RefreshTokenValidTo = refreshTokenExpires;

        var result = await _userManager.UpdateAsync(admin);
        if (!result.Succeeded)
        {
            DeleteRefreshTokenCookie();
            if (result.Errors.Any(error => error.Code == ConcurrencyFailureCode))
            {
                return Result.Fail(AuthConstants.Unauthorized);
            }

            return Result.Fail(result.Errors.Select(error => error.Description));
        }

        AppendRefreshTokenCookie(newRefreshToken, refreshTokenExpires);
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
