using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Delete;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.Queries.Admin.Donate.UahBankDetails.GetAll;
using VictoryCenter.WebAPI.Attributes;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class UahBankDetailsController : AuthorizedApiController
{
    [HttpPost]
    [StrictJson]
    public async Task<IActionResult> CreateUahBankDetails([FromBody] CreateUahBankDetailsDto createUahBankDetailsDto)
    {
        return HandleResult(await Mediator.Send(new CreateUahBankDetailsCommand(createUahBankDetailsDto)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUahBankDetails()
    {
        return HandleResult(await Mediator.Send(new GetAllUahBankDetailsQuery()));
    }

    [HttpPut]
    [Route("{id:long}")]
    [StrictJson]
    public async Task<IActionResult> UpdateUahBankDetails([FromBody] UpdateUahBankDetailsDto updateUahBankDetailsDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateUahBankDetailsCommand(updateUahBankDetailsDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteUahBankDetails(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteUahBankDetailsCommand(id)));
    }
}
