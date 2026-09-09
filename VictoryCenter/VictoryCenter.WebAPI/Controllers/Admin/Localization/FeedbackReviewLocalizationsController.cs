using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;
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
}
