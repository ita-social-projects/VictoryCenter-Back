using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.EventNews;

public record EventNewsCategoryLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Name { get; init; } = null!;
}
