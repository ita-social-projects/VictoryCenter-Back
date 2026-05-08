using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

public record CreateHistorySectionContentLocalizationDto : ILocalizationIdentity, IHistoryContentLocalization
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }

    public string? Title { get; init; }
    public string? Description { get; init; }
}
