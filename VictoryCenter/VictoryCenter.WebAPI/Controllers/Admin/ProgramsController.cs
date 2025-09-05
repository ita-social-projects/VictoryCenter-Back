using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Programs.Create;
using VictoryCenter.BLL.Commands.Admin.Programs.Delete;
using VictoryCenter.BLL.Commands.Admin.Programs.Update;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.Queries.Admin.Programs.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.Programs.GetById;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ProgramsController : AuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetFilteredPrograms([FromQuery] ProgramsFilterDto requestDto)
    {
        return HandleResult(await Mediator.Send(new GetProgramsByFiltersQuery(requestDto)));
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
