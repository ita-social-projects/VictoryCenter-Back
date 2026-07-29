namespace VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;

public record AdminEventNewsCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
}
