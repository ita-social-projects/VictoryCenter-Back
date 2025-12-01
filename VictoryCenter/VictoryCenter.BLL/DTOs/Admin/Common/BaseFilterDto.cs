using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Common;

public record BaseFilterDto
{
    public int? Offset { get; init; }

    public int? Limit { get; init; }

    public Status? Status { get; init; }
}
