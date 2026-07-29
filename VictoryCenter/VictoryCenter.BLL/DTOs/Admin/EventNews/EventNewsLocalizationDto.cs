using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
}
