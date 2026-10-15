using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageEthicsSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyLandingPageEthicsSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<HippotherapyLandingPageEthicsSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHippotherapyLandingPageEthicsSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyLandingPageEthicsSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHippotherapyLandingPageEthicsSectionLocalization(
        [FromBody] CreateHippotherapyLandingPageEthicsSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyLandingPageEthicsSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyLandingPageEthicsSectionLocalization(
        [FromBody] UpdateHippotherapyLandingPageEthicsSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateHippotherapyLandingPageEthicsSectionLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteHippotherapyLandingPageEthicsSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHippotherapyLandingPageEthicsSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
