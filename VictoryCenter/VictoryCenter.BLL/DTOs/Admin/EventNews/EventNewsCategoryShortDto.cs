namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsCategoryShortDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
}
