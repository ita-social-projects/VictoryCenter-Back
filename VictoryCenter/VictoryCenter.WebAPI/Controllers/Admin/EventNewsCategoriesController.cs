using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.Queries.Admin.EventNewsCategories.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class EventNewsCategoriesController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminEventNewsCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllEventNewsCategoriesQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdminEventNewsCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEventNewsCategoryDto category)
    {
        return HandleResult(await Mediator.Send(new CreateEventNewsCategoryCommand(category)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(AdminEventNewsCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEventNewsCategoryDto category)
    {
        return HandleResult(await Mediator.Send(new UpdateEventNewsCategoryCommand(id, category)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteEventNewsCategoryCommand(id)));
    }
}
