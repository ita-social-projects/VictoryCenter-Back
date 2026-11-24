using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.BLL.Queries.Public.PdfSection.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class PdfSectionController : PublicApiController
{
    [HttpGet("public")]
    [ProducesResponseType(typeof(PublicPdfSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicPdfSection()
    {
        return HandleResult(await Mediator.Send(new GetPublicPdfSectionQuery()));
    }
}
