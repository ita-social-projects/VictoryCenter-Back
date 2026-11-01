using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;
public interface IHippotherapyLandingPageResponseFactory
{
    HippotherapyLandingPageDto Create(DAL.Entities.HippotherapyLandingPage page);
}
