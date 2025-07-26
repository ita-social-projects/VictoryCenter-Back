using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Payment.Common;

namespace VictoryCenter.BLL.Commands.Payment.Common;

public record PaymentCommand(PaymentRequestDto Request) : IRequest<Result<PaymentResponseDto>>;
