using FluentResults;
using VictoryCenter.BLL.DTOs.Payment.Donation;

namespace VictoryCenter.BLL.Interfaces.PaymentService;

public interface IDonationService
{
    Task<Result<DonationResponseDto>> CreateDonation(DonationRequestDto request, CancellationToken cancellationToken);
}
