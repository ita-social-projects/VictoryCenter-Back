using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

[Auditable]
public abstract class ProgramSectionContent : IEntity, ITranslatedEntity<ProgramSectionContentLocalization>
{
    public long Id { get; set; }

    [Audited]
    public long SectionId { get; set; }

    [Audited]
    public ContentType ContentType { get; set; }

    [Audited]
    public int Order { get; set; }

    [Audited]
    public int? GroupIndex { get; set; }

    public HippotherapyProgramSection Section { get; set; } = null!;

    public ICollection<ProgramSectionContentLocalization> Localizations { get; set; } = [];
}
