using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

public class HippotherapyLandingPageParticipantCardLocalizationItemDto
{
    public long CardId { get; init; }

    public string Description { get; set; } = null!;

    public TranslationStatus? TranslationStatus { get; init; }
}
