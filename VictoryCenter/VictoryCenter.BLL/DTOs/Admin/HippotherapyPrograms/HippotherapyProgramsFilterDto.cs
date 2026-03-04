using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record HippotherapyProgramsFilterDto : BaseFilterDto, ITranslationStatusFilterDto
{
    public List<long>? CategoryId { get; init; }
    public TranslationStatusFilter? TranslationStatusFilter { get; set; }
}
