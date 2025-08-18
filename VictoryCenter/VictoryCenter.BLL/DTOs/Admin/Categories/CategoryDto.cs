namespace VictoryCenter.BLL.DTOs.Admin.Categories;

public record CategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}
