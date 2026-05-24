using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Queries.Public.HistorySections.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class HistoryController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<HistorySectionDto>))]
    public async Task<IActionResult> GetSections()
    {
        return HandleResult(await Mediator.Send(new GetAllHistorySectionsQuery()));
    }
}
