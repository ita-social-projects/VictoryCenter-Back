using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class TeamCategoryLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(TeamCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTeamCategoryLocalization([FromBody] CreateTeamCategoryLocalizationDto createTeamCategoryLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamCategoryLocalizationCommand(createTeamCategoryLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(TeamCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeamCategoryLocalization(
        [FromBody] UpdateTeamCategoryLocalizationDto updateTeamCategoryLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamCategoryLocalizationCommand(updateTeamCategoryLocalizationDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteTeamCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeamCategoryLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamCategoryLocalizationCommand(EntityId, LanguageId)));
    }
}
