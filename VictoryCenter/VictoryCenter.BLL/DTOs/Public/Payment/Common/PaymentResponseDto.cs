namespace VictoryCenter.BLL.DTOs.Public.Payment.Common;

public record PaymentResponseDto
{
    public string PaymentUrl { get; init; } = null!;
}
