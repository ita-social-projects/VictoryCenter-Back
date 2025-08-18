namespace VictoryCenter.BLL.DTOs.Admin.Categories;

public record CreateCategoryDto
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}
