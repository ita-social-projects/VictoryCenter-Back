namespace VictoryCenter.BLL.DTOs.Categories;

public record CreateCategoryDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
