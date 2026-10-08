using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageQuoteSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HippotherapyLandingPageQuoteSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<HippotherapyLandingPageQuoteSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHippotherapyLandingPageQuoteSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HippotherapyLandingPageQuoteSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHippotherapyLandingPageQuoteSectionLocalization(
        [FromBody] CreateHippotherapyLandingPageQuoteSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HippotherapyLandingPageQuoteSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHippotherapyLandingPageQuoteSectionLocalization(
        [FromBody] UpdateHippotherapyLandingPageQuoteSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteHippotherapyLandingPageQuoteSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHippotherapyLandingPageQuoteSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
