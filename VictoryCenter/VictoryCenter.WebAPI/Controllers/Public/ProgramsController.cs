using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Queries.Programs.GetPublished;

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
