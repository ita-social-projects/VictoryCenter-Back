using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;

public interface IHippotherapyLandingPageScientificReferenceLocalizationUpdater
{
    Task<Result<List<HippotherapyLandingPageScientificReferenceLocalizationItemDto>>> UpsertReferencesAsync(
        HippotherapyLandingPageScientificReferencesSection section,
        List<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto> references,
        long languageId);
}
