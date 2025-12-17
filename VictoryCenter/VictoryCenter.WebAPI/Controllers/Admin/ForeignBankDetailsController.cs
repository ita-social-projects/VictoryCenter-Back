using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Delete;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Queries.Admin.Donate.ForeignBankDetails.GetAll;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Attributes;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ForeignBankDetailsController : AuthorizedApiController
{
    [HttpPost]
    [StrictJson]
    public async Task<IActionResult> CreateForeignBankDetails([FromBody] CreateForeignBankDetailsDto createForeignBankDetailsDto)
    {
        return HandleResult(await Mediator.Send(new CreateForeignBankDetailsCommand(createForeignBankDetailsDto)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllForeignBankDetails(BankCurrency currency)
    {
        return HandleResult(await Mediator.Send(new GetAllForeignBankDetailsQuery(currency)));
    }

    [HttpPut]
    [Route("{id:long}")]
    [StrictJson]
    public async Task<IActionResult> UpdateForeignBankDetails([FromBody] UpdateForeignBankDetailsDto updateForeignBankDetailsDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateForeignBankDetailsCommand(updateForeignBankDetailsDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteForeignBankDetails(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteForeignBankDetailsCommand(id)));
    }
}
