using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VictoryCenter.BLL.Commands.Donations.CreateForeign;
using VictoryCenter.BLL.Commands.Donations.CreateUah;
using VictoryCenter.BLL.DTOs.Donations.ForeignBank;
using VictoryCenter.BLL.DTOs.Donations.UahBank;
using VictoryCenter.BLL.Queries.Donations.GetAll;
using VictoryCenter.BLL.Queries.Donations.GetByCurrency;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.WebAPI.Controllers.Donations;

[Authorize]
public class DonationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllDonations()
    {
        return HandleResult(await Mediator.Send(new GetAllDonationsQuery()));
    }

    [HttpGet("{currency}")]
    public async Task<IActionResult> GetByCurrency(string currency)
    {
        if (!Enum.TryParse<Currency>(currency, true, out var parsed))
        {
            var problemsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var badRequestDetails = problemsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Invalid currency code");
            return BadRequest(badRequestDetails);
        }

        return HandleResult(await Mediator.Send(new GetByCurrencyQuery(parsed)));
    }

    [HttpPost("bank-details/uah")]
    public async Task<ActionResult> CreateUahBankDetails([FromBody] CreateUahBankDetailsDto request)
    {
        return HandleResult(await Mediator.Send(new CreateUahBankDetailsCommand(request)));
    }

    [HttpPost("bank-details/foreign")]
    public async Task<ActionResult> CreateForeignBankDetails([FromBody] CreateForeignBankDetailsDto request)
    {
        return HandleResult(await Mediator.Send(new CreateForeignBankDetailsCommand(request)));
    }
}
