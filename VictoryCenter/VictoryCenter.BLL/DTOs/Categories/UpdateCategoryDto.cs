namespace VictoryCenter.BLL.DTOs.Categories;

public record UpdateCategoryDto : CreateCategoryDto
{
    public required long Id { get; set; }
}
