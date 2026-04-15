namespace VictoryCenter.BLL.DTOs.Admin.MainPartners;

public abstract record BaseMainPartnersDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
