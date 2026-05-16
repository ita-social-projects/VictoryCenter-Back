using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HistoryController : AuthorizedApiController
{
    [HttpPut]
    public async Task<IActionResult> UpdateSections([FromBody] List<UpdateHistorySectionDto> updateSections)
    {
        return HandleResult(await Mediator.Send(new UpdateHistorySectionsCommand(updateSections)));
    }
}
