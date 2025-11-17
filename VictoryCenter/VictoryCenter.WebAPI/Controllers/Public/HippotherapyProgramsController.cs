using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class HippotherapyProgramsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedHippotherapyProgramDto>))]
    public async Task<IActionResult> GetPublishedPrograms()
    {
        return HandleResult(await Mediator.Send(new GetPublishedProgramsQuery()));
    }
}
