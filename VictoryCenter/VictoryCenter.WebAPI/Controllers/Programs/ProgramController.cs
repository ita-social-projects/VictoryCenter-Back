using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Commands.Programs.Create;
using VictoryCenter.BLL.Commands.Programs.Delete;
using VictoryCenter.BLL.Commands.Programs.Update;
using VictoryCenter.BLL.Queries.Programs.GetByFilters;
using VictoryCenter.BLL.Queries.Programs.GetById;

namespace VictoryCenter.WebAPI.Controllers.Programs;

[Authorize]
public class ProgramController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetFilteredPrograms([FromQuery] ProgramFilterRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new GetByFiltersQuery(requestDto)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProgram([FromBody] CreateProgramDto createProgramDto)
    {
        return HandleResult(await Mediator.Send(new CreateProgramCommand(createProgramDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgram(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteProgramCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgram([FromBody] UpdateProgramDto updateProgramDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateProgramCommand(updateProgramDto, id)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgramDto))]
    public async Task<IActionResult> GetProgram([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new GetProgramByIdQuery(id)));
    }
}
