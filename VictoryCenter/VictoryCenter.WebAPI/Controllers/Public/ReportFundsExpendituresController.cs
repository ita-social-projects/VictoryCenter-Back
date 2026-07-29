using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;
using VictoryCenter.BLL.Queries.Public.ReportFundsExpenditures.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class ReportFundsExpendituresController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PublishedReportFundsExpendituresDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublishedReportFundsExpenditures(
        [FromQuery] long? languageId)
    {
        return HandleResult(await Mediator.Send(
            new GetPublishedReportFundsExpendituresQuery(languageId)));
    }
}
