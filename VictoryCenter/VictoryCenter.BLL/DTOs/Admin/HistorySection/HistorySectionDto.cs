using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record HistorySectionDto
{
    public long Id { get; init; }
    public HistorySectionTemplate Template { get; init; }
    public int Order { get; init; }
    public List<HistorySectionContentDto> Contents { get; init; } = [];
}
