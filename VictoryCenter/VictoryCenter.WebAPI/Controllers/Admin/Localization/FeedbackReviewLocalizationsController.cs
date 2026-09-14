using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.BLL.Queries.Admin.Localization.FeedbackReviews.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class FeedbackReviewLocalizationsController : AuthorizedApiController
{
    [HttpGet("entityId/{entityId:long}")]
    [ProducesResponseType(typeof(List<FeedbackReviewLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEntityId(long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetFeedbackReviewLocalizationsByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FeedbackReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackReviewLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new CreateFeedbackReviewLocalizationCommand(localization)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(FeedbackReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long entityId,
        long languageId,
        [FromBody] UpdateFeedbackReviewLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new UpdateFeedbackReviewLocalizationCommand(entityId, languageId, localization)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteFeedbackReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long entityId, long languageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteFeedbackReviewLocalizationCommand(entityId, languageId)));
    }
}
