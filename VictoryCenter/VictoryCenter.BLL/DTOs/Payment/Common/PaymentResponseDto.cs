namespace VictoryCenter.BLL.DTOs.Payment.Common;

public record PaymentResponseDto
{
    public string PaymentUrl { get; init; } = null!;
}
