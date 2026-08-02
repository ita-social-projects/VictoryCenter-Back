using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Interfaces.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
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

        if (partners.Count == 0)
        {
            return Result.Ok(new List<PartnerLocalizationItemDto>());
        }

        var partnerIds = partners.Select(p => p.PartnerId).ToList();

        var existingByPartnerId = (await _repositoryWrapper.PartnerLocalizationsRepository
            .GetAllAsync(new QueryOptions<PartnerLocalization>
            {
                Filter = l => l.LanguageId == languageId && partnerIds.Contains(l.EntityId),
                AsNoTracking = true
            }))
            .ToDictionary(l => l.EntityId);

        var entities = partners.Select(dto =>
        {
            var entity = _mapper.Map<PartnerLocalization>(dto);
            entity.LanguageId = languageId;

            if (existingByPartnerId.TryGetValue(entity.EntityId, out var existing))
            {
                entity.TranslationStatus = TranslationStatus.Relevant;
                entity.CreatedAt = existing.CreatedAt;
            }
            else
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            return entity;
        }).ToList();

        var toUpdate = entities.Where(e => existingByPartnerId.ContainsKey(e.EntityId)).ToList();
        var toCreate = entities.Where(e => !existingByPartnerId.ContainsKey(e.EntityId)).ToList();

        if (toUpdate.Count > 0)
        {
            await _partnerLocalizationService.TrackEntityLocalizationAsync(toUpdate, isUpdate: true);
        }

        if (toCreate.Count > 0)
        {
            await _partnerLocalizationService.TrackEntityLocalizationAsync(toCreate, isUpdate: false);
        }

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new InvalidOperationException();
        }

        var results = entities.Select(e => _mapper.Map<PartnerLocalizationItemDto>(e)).ToList();

        return Result.Ok(results);
    }
}
