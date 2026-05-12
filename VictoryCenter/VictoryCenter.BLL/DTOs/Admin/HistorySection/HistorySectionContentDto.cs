using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HistorySection;

public record HistorySectionContentDto
{
    public long Id { get; init; }
    public ContentType ContentType { get; init; }
    public int Order { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public ImageDto? Image { get; init; }
    public List<HistorySectionContentLocalizationDto> Localizations { get; init; } = [];
}
