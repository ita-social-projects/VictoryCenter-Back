using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageIntroSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyLandingPageIntroSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<HippotherapyLandingPageIntroSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHippotherapyLandingPageIntroSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetHippotherapyLandingPageIntroSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyLandingPageIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHippotherapyLandingPageIntroSectionLocalization(
        [FromBody] CreateHippotherapyLandingPageIntroSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateHippotherapyLandingPageIntroSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyLandingPageIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyLandingPageIntroSectionLocalization(
        [FromBody] UpdateHippotherapyLandingPageIntroSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteHippotherapyLandingPageIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHippotherapyLandingPageIntroSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteHippotherapyLandingPageIntroSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
