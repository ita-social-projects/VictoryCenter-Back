using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;
using VictoryCenter.BLL.Queries.Public.HypotherapyPrograms.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class HypotherapyProgramsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedHypotherapyProgramDto>))]
    public async Task<IActionResult> GetPublishedPrograms()
    {
        return HandleResult(await Mediator.Send(new GetPublishedProgramsQuery()));
    }
}
