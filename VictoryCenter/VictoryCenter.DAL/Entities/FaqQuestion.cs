using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class FaqQuestion : BaseEntity, ITranslatedEntity<FaqQuestionLocalization>
{
    [Audited]
    public string QuestionText { get; set; } = null!;

    [Audited]
    public string? AnswerText { get; set; }

    [Audited]
    public Status Status { get; set; }

    public ICollection<FaqPlacement> Placements { get; set; } = [];

    public ICollection<FaqQuestionLocalization> Localizations { get; set; } = [];
}
