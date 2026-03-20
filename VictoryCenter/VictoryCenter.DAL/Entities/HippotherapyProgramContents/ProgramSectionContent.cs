using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public abstract class ProgramSectionContent : IEntity, ITranslatedEntity<ProgramSectionContentLocalization>
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public int Order { get; set; }

    public int? GroupIndex { get; set; }

    public HippotherapyProgramSection Section { get; set; } = null!;

    public ICollection<ProgramSectionContentLocalization> Localizations { get; set; } = [];
}
