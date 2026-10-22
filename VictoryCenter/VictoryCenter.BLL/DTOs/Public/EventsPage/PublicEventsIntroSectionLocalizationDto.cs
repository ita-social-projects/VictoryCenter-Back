using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.EventsPage;

public record PublicEventsIntroSectionLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;

    public string? EventsBlockTitle { get; init; }

    public string? PageDescription { get; init; }
}
