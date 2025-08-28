using FluentResults;
using VictoryCenter.BLL.DTOs.Payment.Common;

namespace VictoryCenter.BLL.Interfaces.PaymentService;

public interface IPaymentService
{
    Task<Result<PaymentResponseDto>> CreatePayment(PaymentRequestDto request, CancellationToken cancellationToken);
}
