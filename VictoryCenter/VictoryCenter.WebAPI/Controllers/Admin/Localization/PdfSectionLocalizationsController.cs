using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class PdfSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PdfSectionLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfSectionLocalizations()
    {
        return HandleResult(await Mediator.Send(new GetPdfSectionLocalizationsQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PdfSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePdfSectionLocalization(
        [FromBody] CreatePdfSectionLocalizationDto createPdfSectionLocalizationDto)
    {
        return HandleResult(await Mediator.Send(
            new CreatePdfSectionLocalizationCommand(createPdfSectionLocalizationDto)));
    }

    [HttpPut("{languageId:long}")]
    [ProducesResponseType(typeof(PdfSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePdfSectionLocalization(
        [FromBody] UpdatePdfSectionLocalizationDto updatePdfSectionLocalizationDto,
        [FromRoute(Name = "languageId")] long languageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdatePdfSectionLocalizationCommand(updatePdfSectionLocalizationDto, languageId)));
    }
}
