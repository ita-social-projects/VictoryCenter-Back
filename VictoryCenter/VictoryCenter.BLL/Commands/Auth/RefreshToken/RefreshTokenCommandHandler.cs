using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Admin> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<Admin> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenRetrieved = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        if (!refreshTokenRetrieved || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Fail("Refresh token is not present");
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(refreshToken);
        if (principalResult.IsFailed)
        {
            return Result.Fail(principalResult.Errors[0].Message);
        }

        var email = principalResult.Value.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            return Result.Fail("Invalid token");
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            return Result.Fail("Admin with given email was not found");
        }

        if (admin.RefreshToken != refreshToken || admin.RefreshTokenValidTo <= DateTime.UtcNow)
        {
            return Result.Fail("Refresh token is invalid");
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var newRefreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, admin.Email!)]);

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(AuthConstants.RefreshTokenLifeTime),
            Path = "/api/auth/refresh-token"
        });

        admin.RefreshToken = newRefreshToken;
        admin.RefreshTokenValidTo = DateTime.UtcNow.Add(AuthConstants.RefreshTokenLifeTime);

        var result = await _userManager.UpdateAsync(admin);

        return result.Succeeded
            ? Result.Ok(new AuthResponse(accessToken))
            : Result.Fail(result.Errors.First().Description);
    }
}
