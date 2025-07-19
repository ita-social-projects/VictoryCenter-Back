using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Interfaces.PaymentService;

namespace VictoryCenter.WebAPI.Controllers.Donations;

public class PaymentsController : BaseApiController
{
    private readonly IDonationService _donationService;

    public PaymentsController(IDonationService donationService)
    {
        _donationService = donationService;
    }

    [HttpPost("donate")]
    public async Task<IActionResult> Donate([FromForm] DonationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _donationService.CreateDonation(request, cancellationToken);
        if (result.IsSuccess)
        {
            return Redirect(result.Value.PaymentUrl);
        }

        return BadRequest("Unable to conduct donation");
    }
}
