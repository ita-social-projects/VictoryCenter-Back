namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record UpdateHistorySectionDto : CreateHistorySectionDto
{
    public long? Id { get; init; }
}
