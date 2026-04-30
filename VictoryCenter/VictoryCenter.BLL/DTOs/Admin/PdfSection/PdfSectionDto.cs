using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.DTOs.Admin.PdfSection;

public class PdfSectionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<PdfSectionLocalizationDto> Localizations { get; set; } = [];
}
