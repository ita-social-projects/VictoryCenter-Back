using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Public.Auth.Login;
using VictoryCenter.BLL.Commands.Public.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Public.Auth;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    public async Task<IActionResult> LoginAsync(LoginRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new LoginCommand(requestDto)));
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        return HandleResult(await Mediator.Send(new RefreshTokenCommand()));
    }
}
