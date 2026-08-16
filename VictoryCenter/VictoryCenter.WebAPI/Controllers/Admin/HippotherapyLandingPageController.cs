using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HippotherapyLandingPageController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(HippotherapyLandingPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHippotherapyLandingPage()
        => HandleResult(await Mediator.Send(new GetHippotherapyLandingPageQuery()));
}
