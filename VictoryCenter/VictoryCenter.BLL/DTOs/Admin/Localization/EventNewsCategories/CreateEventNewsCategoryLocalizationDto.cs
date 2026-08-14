namespace VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

public record CreateEventNewsCategoryLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public string Name { get; init; } = null!;
}
