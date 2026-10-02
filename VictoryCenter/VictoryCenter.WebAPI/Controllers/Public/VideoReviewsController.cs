using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;
using VictoryCenter.BLL.Queries.Public.VideoReviews.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class VideoReviewsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedVideoReviewDto>))]
    public async Task<IActionResult> GetPublishedVideoReviews()
    {
        return HandleResult(await Mediator.Send(new GetPublishedVideoReviewsQuery()));
    }
}
