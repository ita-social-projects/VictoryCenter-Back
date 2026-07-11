using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Delete;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Update;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class PdfReportsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(PdfReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePdfReport([FromForm] CreatePdfReportDto request)
    {
        return HandleResult(await Mediator.Send(new CreatePdfReportCommand(request)));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<PdfReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPdfReports([FromQuery] PdfReportFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetAllPdfReportsQuery(filter)));
    }

    [HttpGet("{id}")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfReportById(long id)
    {
        var result = await Mediator.Send(new GetPdfReportByIdQuery(id));

        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        return File(result.Value, "application/pdf", fileDownloadName: null);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PdfReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePdfReport(long id, [FromBody] UpdatePdfReportRequestDto request)
    {
        return HandleResult(await Mediator.Send(new UpdatePdfReportCommand(id, request.Name)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePdfReport(long id)
    {
        return HandleResult(await Mediator.Send(new DeletePdfReportCommand(id)));
    }

    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderPdfReports([FromBody] ReorderPdfReportsDto reorderPdfReportsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderPdfReportsCommand(reorderPdfReportsDto)));
    }
}
