using FluentResults;
using VictoryCenter.BLL.Commands.Public.Payment.Common;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;

namespace VictoryCenter.BLL.Interfaces.PaymentService;

public interface IPaymentFactory
{
    PaymentSystem PaymentSystem { get; }
    IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>> GetRequestHandler();
}
