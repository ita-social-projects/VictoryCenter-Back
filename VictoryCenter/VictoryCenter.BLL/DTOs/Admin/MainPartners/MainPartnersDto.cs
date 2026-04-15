namespace VictoryCenter.BLL.DTOs.Admin.MainPartners;

public record MainPartnersDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
