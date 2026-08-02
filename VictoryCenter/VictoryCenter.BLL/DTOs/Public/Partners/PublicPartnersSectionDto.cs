namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PublicPartnersSectionDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<PublicPartnerDto> Partners { get; init; } = [];
    public List<PublicPartnerSectionLocalizationDto> Localizations { get; init; } = [];
}
