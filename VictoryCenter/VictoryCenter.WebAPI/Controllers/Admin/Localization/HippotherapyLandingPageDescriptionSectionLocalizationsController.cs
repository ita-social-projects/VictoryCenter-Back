using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageDescriptionSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyLandingPageDescriptionSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<HippotherapyLandingPageDescriptionSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHippotherapyLandingPageDescriptionSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetHippotherapyLandingPageDescriptionSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyLandingPageDescriptionSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHippotherapyLandingPageDescriptionSectionLocalization(
        [FromBody] CreateHippotherapyLandingPageDescriptionSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyLandingPageDescriptionSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyLandingPageDescriptionSectionLocalization(
        [FromBody] UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHippotherapyLandingPageDescriptionSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
