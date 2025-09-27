using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class SupportOptionsController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateSupportOptions([FromBody] CreateSupportOptionsDto createSupportOptionsDto)
    {
        return HandleResult(await Mediator.Send(new CreateSupportOptionsCommand(createSupportOptionsDto)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSupportOptions()
    {
        return HandleResult(await Mediator.Send(new GetAllSupportOptionsQuery()));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateSupportOptions([FromBody] UpdateSupportOptionsDto updateSupportOptionsDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateSupportOptionsCommand(updateSupportOptionsDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteSupportOptions(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteSupportOptionsCommand(id)));
    }
}
