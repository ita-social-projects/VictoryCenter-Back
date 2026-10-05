using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class FeedbackHistoriesController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedFeedbackHistoryDto>))]
    public async Task<IActionResult> GetPublishedFeedbackHistories()
    {
        return HandleResult(await Mediator.Send(new GetPublishedFeedbackHistoriesQuery()));
    }
}
