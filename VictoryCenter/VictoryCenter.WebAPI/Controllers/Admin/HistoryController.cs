using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Queries.Admin.HistorySections.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HistoryController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<HistorySectionDto>))]
    public async Task<IActionResult> GetSections()
    {
        return HandleResult(await Mediator.Send(new GetAllHistorySectionsQuery()));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSections([FromBody] List<UpdateHistorySectionDto> updateSections)
    {
        return HandleResult(await Mediator.Send(new UpdateHistorySectionsCommand(updateSections)));
    }
}
