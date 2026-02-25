using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.PdfSection;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.PdfSection;

using PdfSection = Entities.PdfSection;

public class PdfSectionRepository : RepositoryBase<PdfSection>, IPdfSectionRepository
{
    public PdfSectionRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
