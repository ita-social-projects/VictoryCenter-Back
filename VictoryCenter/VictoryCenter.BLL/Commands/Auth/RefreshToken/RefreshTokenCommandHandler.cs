using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Admin> _userManager;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<Admin> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetClaimsFromExpiredToken(request.Request.ExpiredAccessToken);
        var email = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);
        if (email is null)
        {
            return Result.Fail("Invalid token");
        }

        var admin = await _userManager.FindByEmailAsync(email.Value);
        if (admin is null)
        {
            return Result.Fail("Admin with given email was not found");
        }

        if (admin.RefreshToken != request.Request.RefreshToken || admin.RefreshTokenValidTo <= DateTime.UtcNow)
        {
            return Result.Fail("Refresh token is invalid");
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            email
        ]);
        var refreshToken = _tokenService.CreateRefreshToken();

        admin.RefreshToken = refreshToken;
        admin.RefreshTokenValidTo = DateTime.UtcNow.Add(Constants.Authentication.RefreshTokenLifeTime);

        await _userManager.UpdateAsync(admin);

        return Result.Ok(new AuthResponse(accessToken, refreshToken));
    }
}
