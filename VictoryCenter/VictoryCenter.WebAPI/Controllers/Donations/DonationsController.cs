using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VictoryCenter.BLL.Queries.Donations.GetAll;
using VictoryCenter.BLL.Queries.Donations.GetByCurrency;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.WebAPI.Controllers.Donations;

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
}
