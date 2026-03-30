using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.PdfSection.Update;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.BLL.Queries.Admin.PdfSection.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class PdfSectionController : AuthorizedApiController
{
    [HttpGet("pdf-section")]
    [ProducesResponseType(typeof(PdfSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfSection()
    {
        return HandleResult(await Mediator.Send(new GetPdfSectionQuery()));
    }

    [HttpPut]
    [ProducesResponseType(typeof(PdfSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePdfSection([FromBody] PdfSectionDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdatePdfSectionCommand(dto)));
    }
}
