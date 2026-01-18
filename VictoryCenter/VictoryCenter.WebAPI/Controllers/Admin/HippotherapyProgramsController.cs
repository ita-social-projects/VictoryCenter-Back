using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Delete;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetById;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HippotherapyProgramsController : AuthorizedApiController
{
    private readonly ISlugService _slugService;

    public HippotherapyProgramsController(ISlugService slugService)
    {
        _slugService = slugService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredPrograms([FromQuery] HippotherapyProgramsFilterDto requestDto)
    {
        return HandleResult(await Mediator.Send(new GetHippotherapyProgramsByFiltersQuery(requestDto)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProgram([FromBody] CreateHippotherapyProgramDto createProgramDto)
    {
        return HandleResult(await Mediator.Send(new CreateHippotherapyProgramCommand(createProgramDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgram(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteHippotherapyProgramCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgram([FromBody] UpdateHippotherapyProgramDto updateProgramDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateHippotherapyProgramCommand(updateProgramDto, id)));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResult<HippotherapyProgramDto>))]
    public async Task<IActionResult> SearchPrograms([FromQuery] SearchHippotherapyProgramDto searchHippotherapyProgramDto)
    {
        return HandleResult(await Mediator.Send(new SearchHippotherapyProgramsQuery(searchHippotherapyProgramDto)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HippotherapyProgramDto))]
    public async Task<IActionResult> GetProgram([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new GetHippotherapyProgramByIdQuery(id)));
    }

    [HttpGet("test/slug")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GenerateSlug([FromQuery] string name)
    {
        return Ok(_slugService.GenerateSlug(name));
    }
}
