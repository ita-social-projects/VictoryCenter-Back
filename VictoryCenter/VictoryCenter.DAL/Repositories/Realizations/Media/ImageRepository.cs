using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Media;

public class ImageRepository : RepositoryBase<Image>, IImageRepository
{
    public ImageRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
