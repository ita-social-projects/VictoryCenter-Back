using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.BLL.Queries.Admin.Localization.FeedbackHistories.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class FeedbackHistoryLocalizationsController : AuthorizedApiController
{
    [HttpGet("entityId/{entityId:long}")]
    [ProducesResponseType(typeof(List<FeedbackHistoryLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEntityId(long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetFeedbackHistoryLocalizationsByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FeedbackHistoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackHistoryLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new CreateFeedbackHistoryLocalizationCommand(localization)));
    }
}
