namespace VictoryCenter.BLL.DTOs.Payment.Donation;

public record DonationRequestDto
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public bool IsSubscription { get; init; }
    public PaymentSystem PaymentSystem { get; init; }
}
