using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Models;

namespace VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;

public interface IWhoWeAreSectionsRepository : IRepositoryBase<WhoWeAreSection>
{
    Task<List<WhoWeAreSectionPreviewRow>> GetPreviewRowsAsync(CancellationToken cancellationToken = default);
}
