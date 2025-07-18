namespace VictoryCenter.BLL.DTOs.Admin.Categories;

public record CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
