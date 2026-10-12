using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsFilterDto : ITranslationStatusFilterDto
{
    public int? Offset { get; init; }
    public int? Limit { get; init; }
    public long? CategoryId { get; init; }
    public Status? Status { get; init; }
    public TranslationStatusFilter? TranslationStatusFilter { get; set; }
}
