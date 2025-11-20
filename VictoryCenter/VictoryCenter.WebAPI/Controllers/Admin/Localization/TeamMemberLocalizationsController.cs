using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByTeamMemberId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class TeamMemberLocalizationsController : AuthorizedApiController
{
    [HttpGet("member/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTeamMemberId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByTeamMemberIdQuery(id)));
    }

    [HttpGet("lang/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByLanguageIdQuery(id)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TeamMemberLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTeamMemberLocalization([FromBody] CreateTeamMemberLocalizationDto createTeamMemberLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamMemberLocalizationCommand(createTeamMemberLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(TeamMemberLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeamMemberLocalization(
        [FromBody] UpdateTeamMemberLocalizationDto updateTeamMemberLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamMemberLocalizationCommand(updateTeamMemberLocalizationDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeamMemberLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberLocalizationCommand(EntityId, LanguageId)));
    }
}
