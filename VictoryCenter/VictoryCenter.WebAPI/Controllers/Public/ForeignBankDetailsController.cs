using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Public.Donate.ForeignBankDetails.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class ForeignBankDetailsController : BaseApiController
{
    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedForeignBankDetails(BankCurrency currency)
    {
        return HandleResult(await Mediator.Send(new GetPublishedForeignBankDetailsQuery(currency)));
    }
}
