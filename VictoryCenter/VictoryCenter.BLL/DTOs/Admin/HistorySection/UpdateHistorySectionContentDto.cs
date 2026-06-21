namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record UpdateHistorySectionContentDto : CreateHistorySectionContentDto
{
    public long Id { get; init; }
}
