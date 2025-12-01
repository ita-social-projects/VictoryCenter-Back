namespace VictoryCenter.BLL.DTOs.Admin.Common;

public record BaseSearchDto
{
    public string SearchQuery { get; init; } = null!;

    public int? Offset { get; init; }

    public int? Limit { get; init; }
}
