using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.CreatePage;
using VictoryCenter.BLL.DTOs;
using VictoryCenter.BLL.Queries.GetAllPages;

namespace VictoryCenter.Controllers.Pages;

public class PagesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPages()
    {
        return HandleResult(await Mediator.Send(new GetAllPagesQuery()));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePage(CreatePageDto createPageDto)
    {
        return HandleResult(await Mediator.Send(new CreatePageCommand(createPageDto)));
    }
}
