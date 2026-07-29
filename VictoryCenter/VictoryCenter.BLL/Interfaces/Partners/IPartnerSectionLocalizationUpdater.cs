using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Interfaces.Partners;

public interface IPartnerSectionLocalizationUpdater
{
    Task<Result<List<PartnerLocalizationItemDto>>> UpsertPartnersAsync(
        PartnerSection section,
        List<UpdatePartnerLocalizationItemDto> partners,
        long languageId);
}
