using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Create;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Update;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetById;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class FeedbackReviewsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<FeedbackReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFilters([FromQuery] FeedbackReviewsFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetFeedbackReviewsByFiltersQuery(filter)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(FeedbackReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        return HandleResult(await Mediator.Send(new GetFeedbackReviewByIdQuery(id)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FeedbackReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateFeedbackReview([FromBody] CreateFeedbackReviewDto createReviewDto)
    {
        return HandleResult(await Mediator.Send(new CreateFeedbackReviewCommand(createReviewDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(FeedbackReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFeedbackReview(long id, [FromBody] UpdateFeedbackReviewDto updateReviewDto)
    {
        return HandleResult(await Mediator.Send(new UpdateFeedbackReviewCommand(id, updateReviewDto)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFeedbackReview(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteFeedbackReviewCommand(id)));
    }
}
