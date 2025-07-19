using FluentResults;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Donation;

namespace VictoryCenter.BLL.Factories.Donation.Interfaces;

public interface IDonationFactory
{
    PaymentSystem PaymentSystem { get; }
    IDonationCommandHandler<DonationCommand, Result<DonationResponseDto>> GetRequestHandler();
}
