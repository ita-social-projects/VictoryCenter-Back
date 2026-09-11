using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Admin.Auth.Login;
using VictoryCenter.BLL.Commands.Admin.Auth.Logout;
using VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [EnableRateLimiting(RateLimitingPolicyNameConstants.AdminLogin)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    public async Task<IActionResult> LoginAsync(LoginRequestDto requestDto)
    {
        return HandleResult(
            await Mediator.Send(new LoginCommand(requestDto)),
            AuthConstants.InvalidCredentials);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        return HandleResult(await Mediator.Send(new RefreshTokenCommand()));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAsync()
    {
        return HandleResult(await Mediator.Send(new LogoutCommand()));
    }
}
