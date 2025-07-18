namespace VictoryCenter.BLL.DTOs.Pages;

public record PageDto
{
    public long Id { get; set; }

    public string Slug { get; set; } = default!;

    public string Title { get; set; } = default!;
}
