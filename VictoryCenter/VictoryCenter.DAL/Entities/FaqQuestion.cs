using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FaqQuestion
{
    public long Id { get; set; }

    public string QuestionText { get; set; }

    public string AnswerText { get; set; }

    public Status Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<FaqPageQuestion> Pages { get; set; } = [];
}
