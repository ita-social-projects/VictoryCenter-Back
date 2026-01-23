using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

public record FaqQuestionsFilterDto : BaseFilterDto, ITranslationStatusFilterDto
{
    public long? PageId { get; init; }

    public TranslationStatusFilter? TranslationStatusFilter { get; set; }
}
