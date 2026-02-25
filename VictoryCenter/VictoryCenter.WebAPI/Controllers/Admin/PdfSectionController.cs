using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.BLL.Queries.Admin.PdfSectionWithReport;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class PdfSectionController : AuthorizedApiController
{
    [HttpGet("pdf-section")]
    [ProducesResponseType(typeof(PdfSectionWithReportsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfSectionWithReports()
    {
        return HandleResult(await Mediator.Send(new GetPdfSectionWithReportsQuery()));
    }
}
