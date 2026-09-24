using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsFilterDto
{
    public int? Offset { get; init; }
    public int? Limit { get; init; }
    public long? CategoryId { get; init; }
    public Status? Status { get; init; }
}
