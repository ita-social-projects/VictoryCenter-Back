using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Media;

public class PdfReportRepository : RepositoryBase<PdfReport>, IPdfReportRepository
{
    public PdfReportRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
