using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Pages.CreatePage;
using VictoryCenter.BLL.DTOs;
using VictoryCenter.BLL.Queries.Pages.GetAllPages;

namespace VictoryCenter.Controllers.Pages;

public class PagesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPages()
    {
        return HandleResult(await Mediator.Send(new GetAllPagesQuery()));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageDto createPageDto)
    {
        return HandleResult(await Mediator.Send(new CreatePageCommand(createPageDto)));
    }
}
