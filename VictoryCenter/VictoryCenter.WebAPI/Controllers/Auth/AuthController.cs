using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Auth.Login;
using VictoryCenter.BLL.Commands.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.WebAPI.Controllers.Auth;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        return HandleResult(await Mediator.Send(new LoginCommand(request)));
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        return HandleResult(await Mediator.Send(new RefreshTokenCommand()));
    }
}
