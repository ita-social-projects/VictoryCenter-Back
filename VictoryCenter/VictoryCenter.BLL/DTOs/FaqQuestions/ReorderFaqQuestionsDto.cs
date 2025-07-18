namespace VictoryCenter.BLL.DTOs.FaqQuestions;

public class ReorderFaqQuestionsDto
{
    public long PageId { get; init; }
    public List<long> OrderedIds { get; init; } = [];
}
