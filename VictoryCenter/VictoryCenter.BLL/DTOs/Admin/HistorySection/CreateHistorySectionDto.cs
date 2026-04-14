using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record CreateHistorySectionDto
{
    public HistorySectionTemplate Template { get; init; }

    public int Order { get; init; }

    public List<CreateHistorySectionContentDto>? Contents { get; init; } = [];
}
