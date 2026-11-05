using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("donate")]
    [EnableRateLimiting(RateLimitingPolicyNameConstants.InitiateDonation)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Donate([FromForm] PaymentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreatePayment(request, cancellationToken);
        if (result.IsSuccess)
        {
            if (string.IsNullOrWhiteSpace(result.Value.PaymentUrl))
            {
                return BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, PaymentConstants.PaymentUrlIsNotAvailable));
            }

            return Redirect(result.Value.PaymentUrl);
        }

        return HandleResult(result);
    }
}
