using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Interfaces.MainPage;

public interface IMainPageBlocksLocalizationUpdater
{
    Task<Result<(MainAboutUsLocalizationDto? MainAboutUs, MainPartnersLocalizationDto? MainPartners, MainDonationsLocalizationDto? MainDonations)>> UpdateBlocksAsync(
        UpdateMainPageLocalizationDto dto,
        MainPageEntity mainPage,
        long languageId);
}
