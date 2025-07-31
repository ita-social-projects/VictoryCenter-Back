using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<Admin> _userManager;

    public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<Admin> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
        {
            return Result.Fail("Unauthorized");
        }

        var admin = await _userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            return Result.Fail("Not found admin");
        }

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = DateTime.MinValue;

        var updateAdmin = await _userManager.UpdateAsync(admin);

        if (!updateAdmin.Succeeded)
        {
            return Result.Fail("Not updated");
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName);
        return Result.Ok(Unit.Value);
    }
}
