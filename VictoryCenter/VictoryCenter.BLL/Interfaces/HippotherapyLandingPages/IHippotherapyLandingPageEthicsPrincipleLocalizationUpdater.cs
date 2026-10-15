using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;

public interface IHippotherapyLandingPageEthicsPrincipleLocalizationUpdater
{
    Task<Result<List<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>>> UpsertPrinciplesAsync(
        HippotherapyLandingPageEthicsSection section,
        List<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto> principles,
        long languageId);
}
