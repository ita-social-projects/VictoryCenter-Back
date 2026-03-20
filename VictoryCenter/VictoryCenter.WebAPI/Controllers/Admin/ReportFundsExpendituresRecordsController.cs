using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Delete;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetAll;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetSummary;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ReportFundsExpendituresRecordsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportFundsExpendituresRecordDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportFundsExpendituresRecords()
    {
        return HandleResult(await Mediator.Send(new GetAllReportFundsExpendituresRecordsQuery()));
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ReportFundsExpendituresSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportFundsExpendituresSummary()
    {
        return HandleResult(await Mediator.Send(new GetReportFundsExpendituresSummaryQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportFundsExpendituresRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReportFundsExpendituresRecord(
        CreateReportFundsExpendituresRecordDto createReportFundsExpendituresRecordDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateReportFundsExpendituresRecordCommand(createReportFundsExpendituresRecordDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ReportFundsExpendituresRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportFundsExpendituresRecord(
        UpdateReportFundsExpendituresRecordDto updateReportFundsExpendituresRecordDto,
        long id)
    {
        return HandleResult(await Mediator.Send(
            new UpdateReportFundsExpendituresRecordCommand(updateReportFundsExpendituresRecordDto, id)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReportFundsExpendituresRecord(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteReportFundsExpendituresRecordCommand(id)));
    }
}
