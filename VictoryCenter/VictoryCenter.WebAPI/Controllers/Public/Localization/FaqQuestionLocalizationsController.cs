using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByFaqQuestionId;
using VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class FaqQuestionLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<FaqQuestionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFaqQuestionId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByFaqQuestionIdQuery(id)));
    }

    [HttpGet("languageId/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<FaqQuestionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByLanguageIdQuery(id)));
    }
}
