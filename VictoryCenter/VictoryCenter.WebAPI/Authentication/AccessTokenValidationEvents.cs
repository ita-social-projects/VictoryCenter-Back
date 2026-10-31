using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.WebAPI.Authentication;

public class AccessTokenValidationEvents : JwtBearerEvents
{
    private readonly UserManager<AdminUser> _userManager;
    private readonly ITokenService _tokenService;

    public AccessTokenValidationEvents(UserManager<AdminUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
        var securityStampHash = context.Principal?.FindFirstValue(AuthConstants.SecurityStampHashClaimType);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(securityStampHash))
        {
            context.Fail(AuthConstants.InvalidToken);
            return;
        }

        var normalizedEmail = _userManager.NormalizeEmail(email);
        var securityStamp = await _userManager.Users
            .AsNoTracking()
            .Where(admin => admin.NormalizedEmail == normalizedEmail)
            .Select(admin => admin.SecurityStamp)
            .SingleOrDefaultAsync(context.HttpContext.RequestAborted);
        if (!_tokenService.VerifySecurityStampHash(securityStamp, securityStampHash))
        {
            context.Fail(AuthConstants.AccessTokenRevoked);
        }
    }
}
