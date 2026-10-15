using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageEthicsPrincipleLocalizationUpdater : IHippotherapyLandingPageEthicsPrincipleLocalizationUpdater
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<HippotherapyLandingPageEthicsPrinciple, HippotherapyLandingPageEthicsPrincipleLocalization> _principleLocalizationService;

    public HippotherapyLandingPageEthicsPrincipleLocalizationUpdater(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<HippotherapyLandingPageEthicsPrinciple, HippotherapyLandingPageEthicsPrincipleLocalization> principleLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _principleLocalizationService = principleLocalizationService;
    }

    public async Task<Result<List<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>>> UpsertPrinciplesAsync(
        HippotherapyLandingPageEthicsSection section,
        List<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto> principles,
        long languageId)
    {
        var sectionPrincipleIds = section.EthicsPrinciples.Select(r => r.Id).ToHashSet();
        var invalidPrincipleIds = principles
            .Select(r => r.PrincipleId)
            .Where(id => !sectionPrincipleIds.Contains(id))
            .ToList();

        if (invalidPrincipleIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(invalidPrincipleIds, typeof(HippotherapyLandingPageEthicsPrinciple)));
        }

        if (principles.Count == 0)
        {
            return Result.Ok(new List<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>());
        }

        var principleIds = principles.Select(r => r.PrincipleId).ToList();
        var existingByPrincipleId = (await _repositoryWrapper.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageEthicsPrincipleLocalization>
            {
                Filter = l => l.LanguageId == languageId && principleIds.Contains(l.EntityId),
                AsNoTracking = true
            }))
            .ToDictionary(l => l.EntityId);

        var entities = principles.Select(dto =>
        {
            var entity = _mapper.Map<HippotherapyLandingPageEthicsPrincipleLocalization>(dto);
            entity.LanguageId = languageId;

            if (existingByPrincipleId.TryGetValue(entity.EntityId, out var existing))
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

        var toUpdate = entities.Where(e => existingByPrincipleId.ContainsKey(e.EntityId)).ToList();
        var toCreate = entities.Where(e => !existingByPrincipleId.ContainsKey(e.EntityId)).ToList();

        if (toUpdate.Count > 0)
        {
            await _principleLocalizationService.TrackEntityLocalizationAsync(toUpdate, isUpdate: true);
        }

        if (toCreate.Count > 0)
        {
            await _principleLocalizationService.TrackEntityLocalizationAsync(toCreate, isUpdate: false);
        }

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new InvalidOperationException();
        }

        var results = entities.Select(e => _mapper.Map<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>(e)).ToList();
        return Result.Ok(results);
    }
}
