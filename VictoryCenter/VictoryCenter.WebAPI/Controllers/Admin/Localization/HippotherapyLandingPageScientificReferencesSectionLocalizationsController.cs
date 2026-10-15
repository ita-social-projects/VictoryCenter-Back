using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyLandingPageScientificReferencesSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHippotherapyLandingPageScientificReferencesSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyLandingPageScientificReferencesSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHippotherapyLandingPageScientificReferencesSectionLocalization(
        [FromBody] CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyLandingPageScientificReferencesSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyLandingPageScientificReferencesSectionLocalization(
        [FromBody] UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHippotherapyLandingPageScientificReferencesSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
