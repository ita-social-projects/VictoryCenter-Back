using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Queries.Admin.Localization.PartnerSections.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class PartnerSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(PartnerSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartnerSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new GetPartnerSectionLocalizationByLanguageIdQuery(EntityId, LanguageId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PartnerSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePartnerSectionLocalization([FromBody] CreatePartnerSectionLocalizationDto createPartnerSectionLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreatePartnerSectionLocalizationCommand(createPartnerSectionLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(PartnerSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartnerSectionLocalization(
        [FromBody] UpdatePartnerSectionLocalizationDto updatePartnerSectionLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdatePartnerSectionLocalizationCommand(updatePartnerSectionLocalizationDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeletePartnerSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartnerSectionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeletePartnerSectionLocalizationCommand(EntityId, LanguageId)));
    }
}
