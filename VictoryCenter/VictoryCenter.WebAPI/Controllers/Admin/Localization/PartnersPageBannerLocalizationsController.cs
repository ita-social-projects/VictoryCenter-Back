using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Queries.Admin.Localization.PartnersPageBanner.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class PartnersPageBannerLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(PartnersPageBannerLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartnersPageBannerLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new GetPartnersPageBannerLocalizationByLanguageIdQuery(EntityId, LanguageId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PartnersPageBannerLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePartnersPageBannerLocalization([FromBody] CreatePartnersPageBannerLocalizationDto createPartnersPageBannerLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreatePartnersPageBannerLocalizationCommand(createPartnersPageBannerLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(PartnersPageBannerLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartnersPageBannerLocalization(
        [FromBody] UpdatePartnersPageBannerLocalizationDto updatePartnersPageBannerLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdatePartnersPageBannerLocalizationCommand(updatePartnersPageBannerLocalizationDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeletePartnersPageBannerLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartnersPageBannerLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeletePartnersPageBannerLocalizationCommand(EntityId, LanguageId)));
    }
}
