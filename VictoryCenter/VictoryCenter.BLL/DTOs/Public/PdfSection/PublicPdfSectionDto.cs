namespace VictoryCenter.BLL.DTOs.Public.PdfSection;

public record PublicPdfSectionDto
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<PublicPdfSectionLocalizationDto> Localizations { get; init; } = [];
}
