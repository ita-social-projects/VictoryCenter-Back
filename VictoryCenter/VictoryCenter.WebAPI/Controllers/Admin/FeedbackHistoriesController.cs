using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Create;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Delete;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Update;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class FeedbackHistoriesController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FeedbackHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeedbackHistories()
    {
        return HandleResult(await Mediator.Send(new GetAllFeedbackHistoriesQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FeedbackHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFeedbackHistory([FromBody] CreateFeedbackHistoryDto createFeedbackHistoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateFeedbackHistoryCommand(createFeedbackHistoryDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(FeedbackHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFeedbackHistory([FromBody] UpdateFeedbackHistoryDto updateFeedbackHistoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateFeedbackHistoryCommand(updateFeedbackHistoryDto, id)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteFeedbackHistory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteFeedbackHistoryCommand(id)));
    }
}
