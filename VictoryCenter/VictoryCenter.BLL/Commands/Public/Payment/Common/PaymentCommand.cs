using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;

namespace VictoryCenter.BLL.Commands.Public.Payment.Common;

public record PaymentCommand(PaymentRequestDto Request) : IRequest<Result<PaymentResponseDto>>;
