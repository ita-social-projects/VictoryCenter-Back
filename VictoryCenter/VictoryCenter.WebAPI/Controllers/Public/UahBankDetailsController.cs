using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Public.Donate.UahBankDetails.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class UahBankDetailsController : BaseApiController
{
    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedUahBankDetails()
    {
        return HandleResult(await Mediator.Send(new GetPublishedUahBankDetailsQuery()));
    }
}
