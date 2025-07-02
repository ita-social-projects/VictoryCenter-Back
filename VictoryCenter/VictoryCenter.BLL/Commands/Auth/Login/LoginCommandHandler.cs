using System.Security.Claims;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Admin> _userManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginCommandHandler(ITokenService tokenService, UserManager<Admin> userManager, IValidator<LoginCommand> validator, IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var admin = await _userManager.FindByEmailAsync(request.Request.Email);
        if (admin is null)
        {
            return Result.Fail("Admin with given email was not found");
        }

        var result = await _userManager.CheckPasswordAsync(admin, request.Request.Password);
        if (!result)
        {
            return Result.Fail("Incorrect password");
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            new Claim(ClaimTypes.Email, request.Request.Email)
        ]);
        var refreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, request.Request.Email)]);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(AuthConstants.RefreshTokenLifeTime),
            Path = "/api/auth/refresh-token"
        });

        admin.RefreshToken = refreshToken;
        admin.RefreshTokenValidTo = DateTime.UtcNow.Add(AuthConstants.RefreshTokenLifeTime);

        var updateResult = await _userManager.UpdateAsync(admin);

        return !updateResult.Succeeded
            ? Result.Fail(updateResult.Errors.Select(x => x.Description))
            : Result.Ok(new AuthResponse(accessToken));
    }
}
