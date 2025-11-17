using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FaqQuestion : BaseEntity
{
    public string QuestionText { get; set; } = null!;

    public string AnswerText { get; set; } = null!;

    public Status Status { get; set; }

    public ICollection<FaqPlacement> Placements { get; set; } = [];
}
