using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.WhoWeAreSections.GetWhoWeArePage;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class WhoWeArePageController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPage()
    {
        return HandleResult(await Mediator.Send(new GetWhoWeArePageQuery()));
    }
}
