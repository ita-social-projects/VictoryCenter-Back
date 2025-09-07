using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization;
using VictoryCenter.BLL.Queries.Common.Localization.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class LocalizationController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateLocalizationLanguage([FromBody] CreateLocalizationLanguageDto localizationLanguageDto)
    {
        return HandleResult(await Mediator.Send(new CreateLocalizationLanguageCommand(localizationLanguageDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteLocalizationLanguage(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteLocalizationLanguageCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateLocalizationLanguage([FromBody] UpdateLocalizationLanguageDto updateLocalizationLanguageDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateLocalizationLanguageCommand(updateLocalizationLanguageDto, id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetLocalizationLanguages()
    {
        return HandleResult(await Mediator.Send(new GetAllLocalizationLanguagesQuery()));
    }
}
