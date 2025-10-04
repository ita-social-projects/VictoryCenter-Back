namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public abstract record BasePartnerSectionCreateUpdateDto<TPartnersDto>
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<TPartnersDto> Partners { get; init; } = [];
}
