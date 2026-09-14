using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.Queries.Admin.Localization.VideoReviews.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class VideoReviewLocalizationsController : AuthorizedApiController
{
    [HttpGet("entityId/{entityId:long}")]
    [ProducesResponseType(typeof(List<VideoReviewLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEntityId(long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetVideoReviewLocalizationsByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(VideoReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateVideoReviewLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new CreateVideoReviewLocalizationCommand(localization)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(VideoReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long entityId,
        long languageId,
        [FromBody] UpdateVideoReviewLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new UpdateVideoReviewLocalizationCommand(entityId, languageId, localization)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteVideoReviewLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long entityId, long languageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteVideoReviewLocalizationCommand(entityId, languageId)));
    }
}
