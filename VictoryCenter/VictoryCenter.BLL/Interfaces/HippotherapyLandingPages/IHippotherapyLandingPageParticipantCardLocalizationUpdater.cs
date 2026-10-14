using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;

public interface IHippotherapyLandingPageParticipantCardLocalizationUpdater
{
    Task<Result<List<HippotherapyLandingPageParticipantCardLocalizationItemDto>>> UpsertCardsAsync(
        HippotherapyLandingPageParticipantsSection section,
        List<UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto> cards,
        long languageId);
}
