using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.BLL.Queries.Admin.ReportMediaSettings.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ReportController : AuthorizedApiController
{
    [HttpGet("report")]
    [ProducesResponseType(typeof(ReportMediaSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportMediaSettings()
    {
        return HandleResult(await Mediator.Send(new GetReportMediaSettingsQuery()));
    }

    [HttpPut("report")]
    [ProducesResponseType(typeof(ReportMediaSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportMediaSettings([FromBody] UpdateReportMediaSettingsDto updateReportMediaSettingsDto)
    {
        return HandleResult(await Mediator.Send(new UpdateReportMediaSettingsCommand(updateReportMediaSettingsDto)));
    }
}
