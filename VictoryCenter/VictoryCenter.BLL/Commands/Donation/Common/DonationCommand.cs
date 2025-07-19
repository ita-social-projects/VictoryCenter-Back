using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Payment.Donation;

namespace VictoryCenter.BLL.Commands.Donation.Common;

public record DonationCommand(DonationRequestDto Request) : IRequest<Result<DonationResponseDto>>;
