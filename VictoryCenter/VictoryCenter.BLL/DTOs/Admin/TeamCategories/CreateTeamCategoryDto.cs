namespace VictoryCenter.BLL.DTOs.Admin.TeamCategories;

public record CreateTeamCategoryDto
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}
