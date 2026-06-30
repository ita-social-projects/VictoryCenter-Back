using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;
using VictoryCenter.BLL.Queries.Public.PdfReports.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class PdfReportsController : BaseApiController
{
    [HttpGet("languageId/{id:long}")]
    [ProducesResponseType(typeof(PaginationResult<PdfReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPdfReportsByLanguageId(long id, [FromQuery] BaseFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetPublicPdfReportsByLanguageIdQuery(id, filter)));
    }

    [HttpGet("{id:long}/file")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfFile(long id)
    {
        var result = await Mediator.Send(new GetPdfReportByIdQuery(id));

        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        return File(result.Value, "application/pdf", fileDownloadName: null);
    }
}
