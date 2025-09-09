using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.Programs;
using VictoryCenter.BLL.Queries.Public.Programs.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class ProgramsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedProgramDto>))]
    public async Task<IActionResult> GetPublishedPrograms()
    {
        return HandleResult(await Mediator.Send(new GetPublishedProgramsQuery()));
    }
}
