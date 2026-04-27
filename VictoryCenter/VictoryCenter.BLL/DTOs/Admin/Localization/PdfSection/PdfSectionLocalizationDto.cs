using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

public record PdfSectionLocalizationDto
{
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public long LanguageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; set; }
}
