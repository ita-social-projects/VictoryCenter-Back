namespace VictoryCenter.BLL.DTOs.Categories;

public record CategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
