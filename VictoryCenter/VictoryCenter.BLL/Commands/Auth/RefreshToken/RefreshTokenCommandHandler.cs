using System.Security.Claims;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Admin> _userManager;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<Admin> userManager, IValidator<RefreshTokenCommand> validator, IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors[0].ErrorMessage);
        }

        var principalResult = _tokenService.GetClaimsFromExpiredToken(request.Request.ExpiredAccessToken);
        if (principalResult.IsFailed)
        {
            return Result.Fail(principalResult.Errors[0].Message);
        }

        var email = principalResult.Value.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            return Result.Fail("Invalid token");
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            return Result.Fail("Admin with given email was not found");
        }

        var refreshTokenRetrieved = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        if (!refreshTokenRetrieved || admin.RefreshToken != refreshToken || admin.RefreshTokenValidTo <= DateTime.UtcNow)
        {
            return Result.Fail("Refresh token is invalid");
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var newRefreshToken = _tokenService.CreateRefreshToken();

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(Constants.Authentication.RefreshTokenLifeTime),
            Path = "/api/auth/refresh-token"
        });

        admin.RefreshToken = newRefreshToken;
        admin.RefreshTokenValidTo = DateTime.UtcNow.Add(Constants.Authentication.RefreshTokenLifeTime);

        var result = await _userManager.UpdateAsync(admin);

        return result.Succeeded
            ? Result.Ok(new AuthResponse(accessToken, newRefreshToken))
            : Result.Fail(result.Errors.First().Description);
    }
}
