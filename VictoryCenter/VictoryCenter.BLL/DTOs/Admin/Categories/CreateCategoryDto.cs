namespace VictoryCenter.BLL.DTOs.Admin.Categories;

public record CreateCategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
