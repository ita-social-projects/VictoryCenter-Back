namespace VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

public class ReorderFaqQuestionsDto
{
    public long PageId { get; init; }
    public List<long> OrderedIds { get; init; } = [];
}
