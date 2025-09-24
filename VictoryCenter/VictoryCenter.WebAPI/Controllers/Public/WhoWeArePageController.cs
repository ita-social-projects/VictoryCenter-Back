using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Public.WhoWeAre.GetWhoWeArePage;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class WhoWeArePageController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPage()
    {
        return HandleResult(await Mediator.Send(new GetWhoWeArePageQuery()));
    }
}
