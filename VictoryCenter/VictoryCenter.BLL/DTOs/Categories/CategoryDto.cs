namespace VictoryCenter.BLL.DTOs.Categories;

public class CategoryDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
