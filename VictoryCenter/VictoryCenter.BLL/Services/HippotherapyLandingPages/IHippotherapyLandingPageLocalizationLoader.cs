namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public interface IHippotherapyLandingPageLocalizationLoader
{
    Task<HippotherapyLandingPageLocalizationSource> LoadAsync(
        DAL.Entities.HippotherapyLandingPage page, CancellationToken cancellationToken);
}
