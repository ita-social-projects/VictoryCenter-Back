namespace VictoryCenter.BLL.DTOs.Categories;

public record CategoryDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
