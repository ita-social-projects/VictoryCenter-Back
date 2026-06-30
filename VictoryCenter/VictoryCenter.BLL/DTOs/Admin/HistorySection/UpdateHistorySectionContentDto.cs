using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record UpdateHistorySectionContentDto
{
    public long Id { get; init; }
    public ContentType ContentType { get; init; }
    public int Order { get; init; }

    public string? Title { get; init; }
    public string? Description { get; init; }
    public long? ImageId { get; init; }
}
