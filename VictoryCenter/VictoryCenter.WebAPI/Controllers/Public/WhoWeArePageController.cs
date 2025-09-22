using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetWhoWeArePage;
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
