using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FaqQuestion : BaseEntity, ITranslatedEntity<FaqLocalization>
{
    public string QuestionText { get; set; } = null!;

    public string AnswerText { get; set; } = null!;

    public Status Status { get; set; }

    public ICollection<FaqPlacement> Placements { get; set; } = [];

    public ICollection<FaqLocalization> Localizations { get; set; } = [];
}
