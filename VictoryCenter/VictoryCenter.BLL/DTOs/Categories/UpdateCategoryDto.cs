namespace VictoryCenter.BLL.DTOs.Categories;

public record UpdateCategoryDto : CreateCategoryDto
{
    public long Id { get; init; }
}
