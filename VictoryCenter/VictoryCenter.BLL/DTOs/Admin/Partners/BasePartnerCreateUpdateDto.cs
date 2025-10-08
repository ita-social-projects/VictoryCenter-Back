namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public abstract record BasePartnerCreateUpdateDto
{
    public string Description { get; init; } = null!;
    public long ImageId { get; init; }
}
