using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Models;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.DAL.Repositories.Interfaces.MainPage;

public interface IMainPageRepository : IRepositoryBase<MainPageEntity>
{
    Task<MainPageLocalizationRow?> GetLocalizationAsync(long entityId, long languageId, CancellationToken cancellationToken = default);
}
