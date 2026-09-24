using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsFilterDto
{
    public Status? Status { get; init; }
    public int? Offset { get; init; }
    public int? Limit { get; init; }
    public long? CategoryId { get; init; }
}
