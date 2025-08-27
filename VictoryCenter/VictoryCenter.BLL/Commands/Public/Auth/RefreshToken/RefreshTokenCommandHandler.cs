using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Public.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<AdminUser> userManager, IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenRetrieved = _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken);
        if (!refreshTokenRetrieved || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail(AuthConstants.RefreshTokenIsNotPresent);
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        if (principalResult.IsFailed)
        {
            return Result.Fail(principalResult.Errors[0].Message);
        }

        var email = principalResult.Value.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            return Result.Fail(AuthConstants.InvalidToken);
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            return Result.Fail(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        if (admin.RefreshToken != refreshToken || admin.RefreshTokenValidTo <= DateTime.UtcNow)
        {
            return Result.Fail(AuthConstants.RefreshTokenIsInvalid);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var newRefreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, admin.Email!)]);
        var refreshTokenExpires = DateTime.UtcNow.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, newRefreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExpires,
            Path = AuthConstants.RefreshTokenCookiePath
        });

        admin.RefreshToken = newRefreshToken;
        admin.RefreshTokenValidTo = refreshTokenExpires;

        var result = await _userManager.UpdateAsync(admin);

        return result.Succeeded
            ? Result.Ok(new AuthResponseDto(accessToken))
            : Result.Fail(result.Errors.First().Description);
    }
}
