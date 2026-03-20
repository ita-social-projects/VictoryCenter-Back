using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresSettings.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ReportFundsExpendituresSettingsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ReportFundsExpendituresSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportFundsExpendituresSettings()
    {
        return HandleResult(await Mediator.Send(new GetReportFundsExpendituresSettingsQuery()));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ReportFundsExpendituresSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportFundsExpendituresSettings(
        UpdateReportFundsExpendituresSettingsDto updateReportFundsExpendituresSettingsDto)
    {
        return HandleResult(await Mediator.Send(
            new UpdateReportFundsExpendituresSettingsCommand(updateReportFundsExpendituresSettingsDto)));
    }
}
