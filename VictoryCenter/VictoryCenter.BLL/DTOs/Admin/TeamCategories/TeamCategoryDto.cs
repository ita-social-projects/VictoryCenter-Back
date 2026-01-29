using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.DTOs.Admin.TeamCategories;

public record TeamCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public long TeamMembersCount { get; set; }
    public IEnumerable<TeamCategoryLocalizationDto> Localizations { get; init; } = [];
}
