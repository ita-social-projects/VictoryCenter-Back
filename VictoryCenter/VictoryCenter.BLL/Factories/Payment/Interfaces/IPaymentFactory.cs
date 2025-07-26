using FluentResults;
using VictoryCenter.BLL.Commands.Payment.Common;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Common;

namespace VictoryCenter.BLL.Factories.Payment.Interfaces;

public interface IPaymentFactory
{
    PaymentSystem PaymentSystem { get; }
    IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>> GetRequestHandler();
}
