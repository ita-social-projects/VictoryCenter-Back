using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Constants;
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
            if (string.IsNullOrWhiteSpace(result.Value.PaymentUrl))
            {
                return BadRequest(PaymentConstants.PaymentUrlIsNotAvailable);
            }

            return Redirect(result.Value.PaymentUrl);
        }

        return BadRequest(result.Errors[0].Message ?? PaymentConstants.UnableToConductDonation);
    }
}
