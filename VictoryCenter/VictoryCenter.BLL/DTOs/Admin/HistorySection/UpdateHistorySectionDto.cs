using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record UpdateHistorySectionDto
{
    public long Id { get; init; }
    public HistorySectionTemplate Template { get; init; }
    public int Order { get; init; }
    public List<UpdateHistorySectionContentDto>? Contents { get; init; } = [];
}
