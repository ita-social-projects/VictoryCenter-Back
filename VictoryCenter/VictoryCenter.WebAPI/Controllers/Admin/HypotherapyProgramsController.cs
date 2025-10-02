using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Create;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Delete;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetById;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HypotherapyProgramsController : AuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetFilteredPrograms([FromQuery] HypotherapyProgramsFilterDto requestDto)
    {
        return HandleResult(await Mediator.Send(new GetHypotherapyProgramsByFiltersQuery(requestDto)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProgram([FromBody] CreateHypotherapyProgramDto createProgramDto)
    {
        return HandleResult(await Mediator.Send(new CreateHypotherapyProgramCommand(createProgramDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgram(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteHypotherapyProgramCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgram([FromBody] HypotherapyUpdateProgramDto updateProgramDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateHypotherapyProgramCommand(updateProgramDto, id)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HypotherapyProgramDto))]
    public async Task<IActionResult> GetProgram([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new GetHypotherapyProgramByIdQuery(id)));
    }
}
