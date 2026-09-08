using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Queries.Public.HippotherapyLandingPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

[Route("api/HippotherapyPage")]
public class HippotherapyLandingPageController : BaseApiController
{
    [HttpGet("public")]
    [ProducesResponseType(typeof(HippotherapyLandingPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicHippotherapyLandingPage()
        => HandleResult(await Mediator.Send(new GetPublicHippotherapyLandingPageQuery()));
}
