using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class LocalizationLanguageController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(LocalizationLanguageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLocalizationLanguage([FromBody] CreateLocalizationLanguageDto localizationLanguageDto)
    {
        return HandleResult(await Mediator.Send(new CreateLocalizationLanguageCommand(localizationLanguageDto)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocalizationLanguage(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteLocalizationLanguageCommand(id)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(LocalizationLanguageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocalizationLanguage([FromBody] UpdateLocalizationLanguageDto updateLocalizationLanguageDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateLocalizationLanguageCommand(updateLocalizationLanguageDto, id)));
    }
}
