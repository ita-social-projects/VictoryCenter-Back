using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.FAQ;
using VictoryCenter.BLL.Queries.Public.FaqQuestions.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class FaqController : BaseApiController
{
    [HttpGet("published/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedFaqQuestionDto>))]
    public async Task<IActionResult> GetPublishedTeamMembers([FromRoute] string slug)
    {
        return HandleResult(await Mediator.Send(new GetPublishedFaqQuestionsBySlugQuery(slug)));
    }
}
