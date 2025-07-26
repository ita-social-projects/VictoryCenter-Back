namespace VictoryCenter.BLL.DTOs.Categories;

public record CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
