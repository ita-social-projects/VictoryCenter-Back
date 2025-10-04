using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Logout;

public class LogoutCommandHandler : BaseHandler<LogoutCommand, Unit>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AdminUser> _userManager;

    public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AdminUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public override async Task<Unit> HandleRequest(LogoutCommand request, CancellationToken cancellationToken)
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
        {
            throw new Exception(AuthConstants.Unauthorized);
        }

        var admin = await _userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            throw new Exception(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        admin.RefreshToken = null;
        admin.RefreshTokenValidTo = DateTimeOffset.MinValue;

        var updateAdmin = await _userManager.UpdateAsync(admin);

        if (!updateAdmin.Succeeded)
        {
            throw new Exception(AuthConstants.NotUpdated);
        }

        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName);
        return Unit.Value;
    }
}
