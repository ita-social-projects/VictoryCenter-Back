using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Common.Localization.Languages.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class LocalizationLanguageController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LocalizationLanguageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocalizationLanguages()
    {
        return HandleResult(await Mediator.Send(new GetAllLocalizationLanguagesQuery()));
    }
}
