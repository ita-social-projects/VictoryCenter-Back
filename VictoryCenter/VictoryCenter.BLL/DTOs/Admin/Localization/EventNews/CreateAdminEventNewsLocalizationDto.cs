using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

public record CreateAdminEventNewsLocalizationDto : UpdateEventNewsLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
