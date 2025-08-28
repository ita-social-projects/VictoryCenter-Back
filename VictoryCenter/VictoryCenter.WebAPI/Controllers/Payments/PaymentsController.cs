using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;

namespace VictoryCenter.WebAPI.Controllers.Payments;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("donate")]
    public async Task<IActionResult> Donate([FromForm] PaymentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreatePayment(request, cancellationToken);
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
