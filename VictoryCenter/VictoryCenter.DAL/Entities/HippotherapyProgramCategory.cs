using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class HippotherapyProgramCategory : BaseEntity, ITranslatedEntity<HippotherapyProgramCategoryLocalization>
{
    public string Name { get; set; } = null!;
    public ICollection<HippotherapyProgram> Programs { get; set; } = new List<HippotherapyProgram>();
    public ICollection<HippotherapyProgramCategoryLocalization> Localizations { get; set; }
}
