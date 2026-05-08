using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class MainPageLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(MainPageLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMainPageLocalization([FromBody] CreateMainPageLocalizationDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateMainPageLocalizationCommand(dto)));
    }
}
