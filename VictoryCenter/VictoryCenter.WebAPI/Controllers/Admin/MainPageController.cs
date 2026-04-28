using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.MainPage.Create;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.Queries.Admin.MainPage.GetMainPage;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class MainPageController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMainPage()
    {
        return HandleResult(await Mediator.Send(new GetMainPageQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMainPage([FromBody] CreateMainPageDto createMainPageDto)
    {
        return HandleResult(await Mediator.Send(new CreateMainPageCommand(createMainPageDto)));
    }

    [HttpPut]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMainPage([FromBody] UpdateMainPageDto updateMainPageDto)
    {
        return HandleResult(await Mediator.Send(new UpdateMainPageCommand(updateMainPageDto)));
    }
}
