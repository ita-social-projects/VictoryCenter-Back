using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Restore;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class VideoReviewsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<VideoReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool archived = false)
    {
        return HandleResult(await Mediator.Send(new GetAllVideoReviewsQuery(archived)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(VideoReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVideoReviewDto videoReview)
    {
        return HandleResult(await Mediator.Send(new CreateVideoReviewCommand(videoReview)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(VideoReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateVideoReviewDto videoReview)
    {
        return HandleResult(await Mediator.Send(new UpdateVideoReviewCommand(id, videoReview)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteVideoReviewCommand(id)));
    }

    [HttpPost("{id:long}/restore")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restore(long id)
    {
        return HandleResult(await Mediator.Send(new RestoreVideoReviewCommand(id)));
    }
}
