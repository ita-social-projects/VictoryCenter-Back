using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FaqQuestion
{
    public long Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public string AnswerText { get; set; } = null!;

    public Status Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<FaqPlacement> Placements { get; set; } = [];
}
