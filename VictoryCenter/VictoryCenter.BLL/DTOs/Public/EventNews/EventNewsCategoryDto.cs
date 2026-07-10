namespace VictoryCenter.BLL.DTOs.Public.EventNews;

public record EventNewsCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
}
