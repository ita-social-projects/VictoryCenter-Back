namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public interface IHippotherapyLandingPageLocalizationBuilder
{
    HippotherapyLandingPageLocalizationSource Build(DAL.Entities.HippotherapyLandingPage page);
}
