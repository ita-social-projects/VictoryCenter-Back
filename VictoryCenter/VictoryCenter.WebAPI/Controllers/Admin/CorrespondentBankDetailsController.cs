using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Delete;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class CorrespondentBankDetailsController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateCorrespondentBankDetails([FromBody] CreateCorrespondentBankDetailsDto createCorrespondentBankDetailsDto)
    {
        return HandleResult(await Mediator.Send(new CreateCorrespondentBankDetailsCommand(createCorrespondentBankDetailsDto)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateCorrespondentBankDetails([FromBody] UpdateCorrespondentBankDetailsDto updateCorrespondentBankDetailsDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateCorrespondentBankDetailsCommand(updateCorrespondentBankDetailsDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteCorrespondentBankDetails(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteCorrespondentBankDetailsCommand(id)));
    }
}
