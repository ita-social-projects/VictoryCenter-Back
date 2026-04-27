using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

public class PdfSectionLocalizationDto : ILocalizationIdentity
{
    public long EntityId { get; set; }

    public long LanguageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; set; }
}
