using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.PdfSection;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.PdfSection;

public class PdfSectionLocalizationsRepository : RepositoryBase<PdfSectionLocalization>, IPdfSectionLocalizationsRepository
{
    public PdfSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
