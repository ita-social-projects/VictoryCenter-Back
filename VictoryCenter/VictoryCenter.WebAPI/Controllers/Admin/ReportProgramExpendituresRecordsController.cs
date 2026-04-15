using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ReportProgramExpendituresRecordsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(ReportProgramExpendituresRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReportProgramExpenditureRecordAsync(
        CreateReportProgramExpendituresRecordDto createReportProgramExpendituresRecordDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateReportProgramExpendituresRecordCommand(createReportProgramExpendituresRecordDto)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteReportProgramExpenditureRecordAsync([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new DeleteReportProgramExpendituresRecordCommand(id)));
    }
}
