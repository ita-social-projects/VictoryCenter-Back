namespace VictoryCenter.BLL.DTOs.Public.Payment.Common;

public record PaymentRequestDto
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }
    public bool IsSubscription { get; init; }
    public PaymentSystem PaymentSystem { get; init; }
    public string? ReturnUrl { get; init; }
}
