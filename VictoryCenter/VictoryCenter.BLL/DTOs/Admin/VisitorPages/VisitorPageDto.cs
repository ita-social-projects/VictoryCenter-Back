namespace VictoryCenter.BLL.DTOs.Admin.VisitorPages;

public record VisitorPageDto
{
    public long Id { get; set; }

    public string Slug { get; set; } = default!;

    public string Title { get; set; } = default!;
}
