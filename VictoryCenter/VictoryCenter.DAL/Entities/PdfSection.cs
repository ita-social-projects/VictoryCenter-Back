using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class PdfSection : BaseEntity, ITranslatedEntity<PdfSectionLocalization>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public ICollection<PdfSectionLocalization> Localizations { get; set; } = [];
}
