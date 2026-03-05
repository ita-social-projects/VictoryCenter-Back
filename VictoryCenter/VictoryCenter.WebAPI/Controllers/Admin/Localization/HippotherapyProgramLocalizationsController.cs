using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
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

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyProgramLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyProgramLocalization(
        [FromBody] UpdateHippotherapyProgramLocalizationDto updateHippotherapyProgramLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateHippotherapyProgramLocalizationCommand(updateHippotherapyProgramLocalizationDto, EntityId, LanguageId)));
    }
}
