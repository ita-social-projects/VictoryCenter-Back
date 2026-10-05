using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.FeedbackReviews;
using VictoryCenter.BLL.Queries.Public.FeedbackReviews.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class FeedbackReviewsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedFeedbackReviewDto>))]
    public async Task<IActionResult> GetPublishedFeedbackReviews()
    {
        return HandleResult(await Mediator.Send(new GetPublishedFeedbackReviewsQuery()));
    }
}
