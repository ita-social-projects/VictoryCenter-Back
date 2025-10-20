using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;

public class RefreshTokenCommandHandler : BaseHandler<RefreshTokenCommand, AuthResponseDto>
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

    public override async Task<AuthResponseDto> HandleRequest(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new Exception(AuthConstants.Unauthorized);
        }

        var refreshTokenRetrieved = _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var refreshToken);
        if (!refreshTokenRetrieved || string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new Exception(AuthConstants.RefreshTokenIsNotPresent);
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        if (principalResult.IsFailed)
        {
            throw new Exception(principalResult.Errors[0].Message);
        }

        var email = principalResult.Value.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
           throw new Exception(AuthConstants.InvalidToken);
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            throw new Exception(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        if (admin.RefreshToken != refreshToken || admin.RefreshTokenValidTo <= DateTimeOffset.UtcNow)
        {
            throw new Exception(AuthConstants.RefreshTokenIsInvalid);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var newRefreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, admin.Email!)]);
        var refreshTokenExpires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, newRefreshToken, new CookieOptions
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
            ? new AuthResponseDto(accessToken)
            : throw new Exception(result.Errors.First().Description);
    }
}
