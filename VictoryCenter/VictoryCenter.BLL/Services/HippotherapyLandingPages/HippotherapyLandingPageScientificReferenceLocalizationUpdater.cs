using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageScientificReferenceLocalizationUpdater : IHippotherapyLandingPageScientificReferenceLocalizationUpdater
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<HippotherapyLandingPageScientificReference, HippotherapyLandingPageScientificReferenceLocalization> _referenceLocalizationService;

    public HippotherapyLandingPageScientificReferenceLocalizationUpdater(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<HippotherapyLandingPageScientificReference, HippotherapyLandingPageScientificReferenceLocalization> referenceLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _referenceLocalizationService = referenceLocalizationService;
    }

    public async Task<Result<List<HippotherapyLandingPageScientificReferenceLocalizationItemDto>>> UpsertReferencesAsync(
        HippotherapyLandingPageScientificReferencesSection section,
        List<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto> references,
        long languageId)
    {
        var sectionReferenceIds = section.ScientificReferences.Select(r => r.Id).ToHashSet();
        var invalidReferenceIds = references
            .Select(r => r.ReferenceId)
            .Where(id => !sectionReferenceIds.Contains(id))
            .ToList();

        if (invalidReferenceIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(invalidReferenceIds, typeof(HippotherapyLandingPageScientificReference)));
        }

        if (references.Count == 0)
        {
            return Result.Ok(new List<HippotherapyLandingPageScientificReferenceLocalizationItemDto>());
        }

        var referenceIds = references.Select(r => r.ReferenceId).ToList();
        var existingByReferenceId = (await _repositoryWrapper.HippotherapyLandingPageScientificReferenceLocalizationsRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageScientificReferenceLocalization>
            {
                Filter = l => l.LanguageId == languageId && referenceIds.Contains(l.EntityId),
                AsNoTracking = true
            }))
            .ToDictionary(l => l.EntityId);

        var entities = references.Select(dto =>
        {
            var entity = _mapper.Map<HippotherapyLandingPageScientificReferenceLocalization>(dto);
            entity.LanguageId = languageId;

            if (existingByReferenceId.TryGetValue(entity.EntityId, out var existing))
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

        var toUpdate = entities.Where(e => existingByReferenceId.ContainsKey(e.EntityId)).ToList();
        var toCreate = entities.Where(e => !existingByReferenceId.ContainsKey(e.EntityId)).ToList();

        if (toUpdate.Count > 0)
        {
            await _referenceLocalizationService.TrackEntityLocalizationAsync(toUpdate, isUpdate: true);
        }

        if (toCreate.Count > 0)
        {
            await _referenceLocalizationService.TrackEntityLocalizationAsync(toCreate, isUpdate: false);
        }

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new InvalidOperationException();
        }

        var results = entities.Select(e => _mapper.Map<HippotherapyLandingPageScientificReferenceLocalizationItemDto>(e)).ToList();
        return Result.Ok(results);
    }
}
