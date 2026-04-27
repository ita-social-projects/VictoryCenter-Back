namespace VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

public class PdfSectionWithLocalizationsDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<PdfSectionLocalizationDto> Localizations { get; set; } = [];
}
