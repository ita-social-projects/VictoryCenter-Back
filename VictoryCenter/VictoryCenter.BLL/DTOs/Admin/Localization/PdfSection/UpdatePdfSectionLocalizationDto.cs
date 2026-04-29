namespace VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

public record UpdatePdfSectionLocalizationDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
