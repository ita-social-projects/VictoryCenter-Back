using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class WhoWeAreContentLocalizationsController : AuthorizedApiController
{
    [HttpPost("{sectionType}")]
    [ProducesResponseType(typeof(List<WhoWeAreContentLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateWhoWeAreContentLocalization([FromBody] List<CreateWhoWeAreContentLocalizationDto> dtos, SectionType sectionType)
    {
        return HandleResult(await Mediator.Send(new CreateWhoWeAreContentLocalizationCommand(sectionType, dtos)));
    }
}
