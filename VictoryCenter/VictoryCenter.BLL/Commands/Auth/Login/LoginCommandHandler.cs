using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Admin> _userManager;

    public LoginCommandHandler(ITokenService tokenService, UserManager<Admin> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
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
        var refreshToken = _tokenService.CreateRefreshToken();

        admin.RefreshToken = refreshToken;
        admin.RefreshTokenValidTo = DateTime.UtcNow.Add(Constants.Authentication.RefreshTokenLifeTime);

        var updateResult = await _userManager.UpdateAsync(admin);

        return !updateResult.Succeeded
            ? Result.Fail(updateResult.Errors.Select(x => x.Description))
            : Result.Ok(new AuthResponse(accessToken, refreshToken));
    }
}
