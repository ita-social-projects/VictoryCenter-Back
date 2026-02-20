using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class HippotherapyProgramLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{id:long}")]
    [ProducesResponseType(typeof(List<HippotherapyProgramLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntityId(long id)
    {
        return HandleResult(await Mediator.Send(new GetHippotherapyProgramLocalizationByEntityIdQuery(id)));
    }
}
