using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;
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
    [ProducesResponseType(typeof(List<PdfReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPdfReports([FromQuery] BaseFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetAllPdfReportsQuery(filter)));
    }
}
