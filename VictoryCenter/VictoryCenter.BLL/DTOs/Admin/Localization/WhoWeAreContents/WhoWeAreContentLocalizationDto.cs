using System.Text.Json.Serialization;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

public record WhoWeAreContentLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    public TranslationStatus TranslationStatus { get; init; }
}
