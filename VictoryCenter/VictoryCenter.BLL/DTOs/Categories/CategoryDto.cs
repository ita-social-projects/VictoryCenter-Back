namespace VictoryCenter.BLL.DTOs.Categories;

public record CategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
