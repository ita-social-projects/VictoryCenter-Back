namespace VictoryCenter.BLL.DTOs.Admin.Pages;

public record PageDto
{
    public long Id { get; set; }

    public string Slug { get; set; } = default!;

    public string Title { get; set; } = default!;
}
