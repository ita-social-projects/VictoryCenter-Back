using VictoryCenter.BLL.DTOs.Admin.Common;

namespace VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

public record FaqQuestionsFilterDto : BaseFilterDto
{
    public long? PageId { get; init; }
}
