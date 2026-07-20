using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

[Route("api/admin/report-funds-expenditures")]
public class ReportFundsExpendituresPublishController : AuthorizedApiController
{
    [HttpPost("publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish()
    {
        return HandleResult(await Mediator.Send(new PublishReportFundsExpendituresCommand()));
    }
}
