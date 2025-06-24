using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Auth.Login;
using VictoryCenter.BLL.Commands.Auth.RefrehsToken;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.WebAPI.Controllers.Auth;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        return HandleResult(await Mediator.Send(new LoginCommand(request)));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        return HandleResult(await Mediator.Send(new RefreshTokenCommand(request)));
    }
}
