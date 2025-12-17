using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class FaqQuestionLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(FaqQuestionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFaqQuestionLocalization([FromBody] CreateFaqQuestionLocalizationDto createFaqQuestionLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateFaqQuestionLocalizationCommand(createFaqQuestionLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(FaqQuestionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFaqQuestionLocalization(
        [FromBody] UpdateFaqQuestionLocalizationDto updateFaqQuestionLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateFaqQuestionLocalizationCommand(updateFaqQuestionLocalizationDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteFaqQuestionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFaqQuestionLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeleteFaqQuestionLocalizationCommand(EntityId, LanguageId)));
    }
}
