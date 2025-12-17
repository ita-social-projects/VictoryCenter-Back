using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Attributes;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class SupportOptionsController : AuthorizedApiController
{
    [HttpPost]
    [StrictJson]
    public async Task<IActionResult> CreateSupportOptions([FromBody] CreateSupportOptionsDto createSupportOptionsDto)
    {
        return HandleResult(await Mediator.Send(new CreateSupportOptionsCommand(createSupportOptionsDto)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSupportOptions(BankCurrency currency)
    {
        return HandleResult(await Mediator.Send(new GetAllSupportOptionsQuery(currency)));
    }

    [HttpPut]
    [Route("{id:long}")]
    [StrictJson]
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
