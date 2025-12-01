using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Public.Donate.SupportOptions.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class SupportOptionsController : BaseApiController
{
    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedSupportOptions(BankCurrency currency)
    {
        return HandleResult(await Mediator.Send(new GetPublishedSupportOptionsQuery(currency)));
    }
}
