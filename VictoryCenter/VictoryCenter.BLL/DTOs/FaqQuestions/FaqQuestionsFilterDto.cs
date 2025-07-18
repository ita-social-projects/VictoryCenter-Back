using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.FaqQuestions;

public record FaqQuestionsFilterDto : BaseFilterDto
{
    public long? PageId { get; init; }
}
