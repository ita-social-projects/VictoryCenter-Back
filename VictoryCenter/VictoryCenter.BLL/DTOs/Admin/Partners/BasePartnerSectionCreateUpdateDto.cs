namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public abstract record BasePartnerSectionCreateUpdateDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
