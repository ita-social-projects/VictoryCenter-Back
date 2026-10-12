using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;

public interface IHippotherapyLandingPageAdvantageCardLocalizationUpdater
{
    Task<Result<List<HippotherapyLandingPageAdvantageCardLocalizationItemDto>>> UpsertCardsAsync(
        HippotherapyLandingPageAdvantagesSection section,
        List<UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto> cards,
        long languageId);
}
