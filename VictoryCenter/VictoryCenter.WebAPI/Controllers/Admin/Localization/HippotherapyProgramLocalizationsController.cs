using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyProgramLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyProgramLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateHippotherapyProgramLocalization([FromBody] CreateHippotherapyProgramLocalizationDto createHippotherapyProgramLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateHippotherapyProgramLocalizationCommand(createHippotherapyProgramLocalizationDto)));
    }
}
