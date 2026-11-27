using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.PdfSection;

public record PublicPdfSectionLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
