using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Donations.GetAll;

namespace VictoryCenter.WebAPI.Controllers.Donations;

public class DonationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllDonations()
    {
        return HandleResult(await Mediator.Send(new GetAllDonationsQuery()));
    }
}
