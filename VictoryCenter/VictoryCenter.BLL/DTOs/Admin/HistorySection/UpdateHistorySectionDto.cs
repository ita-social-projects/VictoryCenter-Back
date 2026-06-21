namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record UpdateHistorySectionDto : CreateHistorySectionDto
{
    public long Id { get; init; }

    public new List<UpdateHistorySectionContentDto>? Contents { get; init; } = [];
}
