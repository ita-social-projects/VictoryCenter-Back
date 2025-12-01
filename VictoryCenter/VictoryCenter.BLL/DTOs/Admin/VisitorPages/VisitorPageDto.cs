namespace VictoryCenter.BLL.DTOs.Admin.VisitorPages;

public record VisitorPageDto
{
    public long Id { get; init; }

    public string Slug { get; init; } = null!;

    public string Title { get; init; } = null!;
}
