namespace VictoryCenter.BLL.DTOs.Payment.Donation;

public record DonationResponseDto
{
    public string PaymentUrl { get; init; } = null!;
}
