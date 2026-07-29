using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Interfaces.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.Partners;

public class PartnerSectionLocalizationUpdater : IPartnerSectionLocalizationUpdater
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<Partner, PartnerLocalization> _partnerLocalizationService;

    public PartnerSectionLocalizationUpdater(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<Partner, PartnerLocalization> partnerLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _partnerLocalizationService = partnerLocalizationService;
    }

    public async Task<Result<List<PartnerLocalizationItemDto>>> UpsertPartnersAsync(
        PartnerSection section,
        List<UpdatePartnerLocalizationItemDto> partners,
        long languageId)
    {
        var sectionPartnerIds = section.Partners.Select(p => p.Id).ToHashSet();
        var invalidPartnerIds = partners
            .Select(p => p.PartnerId)
            .Where(id => !sectionPartnerIds.Contains(id))
            .ToList();

        if (invalidPartnerIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(invalidPartnerIds, typeof(Partner)));
        }

        var results = new List<PartnerLocalizationItemDto>();

        foreach (var partnerDto in partners)
        {
            var entity = _mapper.Map<PartnerLocalization>(partnerDto);
            entity.LanguageId = languageId;

            var existing = await _repositoryWrapper.GetRepository<PartnerLocalization>()
                .GetFirstOrDefaultAsync(new QueryOptions<PartnerLocalization>
                {
                    Filter = l => l.EntityId == entity.EntityId && l.LanguageId == languageId,
                    AsNoTracking = true
                });

            var upserted = existing is null
                ? await _partnerLocalizationService.CreateEntityLocalizationAsync(entity)
                : await _partnerLocalizationService.UpdateEntityLocalizationAsync(entity);

            results.Add(_mapper.Map<PartnerLocalizationItemDto>(upserted));
        }

        return Result.Ok(results);
    }
}
