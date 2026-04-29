namespace VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

public record CreatePdfSectionLocalizationDto : UpdatePdfSectionLocalizationDto
{
    public long LanguageId { get; init; }
}
