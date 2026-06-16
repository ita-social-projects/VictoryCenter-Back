using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetStatuses;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class MainPageLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(MainPageLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMainPageLocalizationByLanguageId(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new GetMainPageLocalizationByLanguageIdQuery(EntityId, LanguageId)));
    }

    [HttpGet("{entityId:long}/{languageId:long}/statuses")]
    [ProducesResponseType(typeof(List<MainPageTranslationStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMainPageTranslationStatuses(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new GetMainPageTranslationStatusesQuery(EntityId, LanguageId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(MainPageLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMainPageLocalization([FromBody] CreateMainPageLocalizationDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateMainPageLocalizationCommand(dto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(MainPageLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMainPageLocalization(
        [FromBody] UpdateMainPageLocalizationDto dto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateMainPageLocalizationCommand(dto, EntityId, LanguageId)));
    }
}
