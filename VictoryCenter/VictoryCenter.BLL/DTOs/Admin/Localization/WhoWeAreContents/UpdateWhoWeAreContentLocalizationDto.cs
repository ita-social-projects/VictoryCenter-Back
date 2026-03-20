using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

public record UpdateWhoWeAreContentLocalizationDto : ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }
}
