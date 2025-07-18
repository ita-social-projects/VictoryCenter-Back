namespace VictoryCenter.BLL.DTOs.Admin.Categories;

public record UpdateCategoryDto : CreateCategoryDto
{
    public long Id { get; set; }
}
