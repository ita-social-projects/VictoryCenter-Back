using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record MainPageTranslationStatusDto
{
    public MainPageLocalizationBlock Block { get; init; }
    public long? EntityId { get; init; }
    public long LanguageId { get; init; }
    public TranslationStatus? TranslationStatus { get; init; }
}
