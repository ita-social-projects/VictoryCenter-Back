using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

namespace VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;

public record AdminEventNewsCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public int RelatedEventNewsCount { get; init; }
    public List<AdminEventNewsCategoryLocalizationDto> Localizations { get; init; } = [];
}
