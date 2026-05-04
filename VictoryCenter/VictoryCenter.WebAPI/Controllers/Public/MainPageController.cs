using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.Queries.Public.MainPage.GetMainPage;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class MainPageController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMainPage()
    {
        return HandleResult(await Mediator.Send(new GetMainPageQuery()));
    }
}
