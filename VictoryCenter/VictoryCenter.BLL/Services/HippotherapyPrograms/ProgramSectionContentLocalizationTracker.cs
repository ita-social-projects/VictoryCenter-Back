using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyPrograms;

public class ProgramSectionContentLocalizationTracker : IProgramSectionContentLocalizationTracker
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> _contentLocalizationService;
    private readonly TimeProvider _timeProvider;

    public ProgramSectionContentLocalizationTracker(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> contentLocalizationService,
        TimeProvider timeProvider)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _contentLocalizationService = contentLocalizationService;
        _timeProvider = timeProvider;
    }

    public async Task TrackAsync(
        IEnumerable<UpdateHippotherapyProgramSectionContentLocalizationDto> contentDtos,
        long languageId)
    {
        var contentDtoList = contentDtos.ToList();
        var contentLocalizations = _mapper.Map<List<ProgramSectionContentLocalization>>(contentDtoList);

        for (int i = 0; i < contentLocalizations.Count; i++)
        {
            contentLocalizations[i].EntityId = contentDtoList[i].EntityId;
            contentLocalizations[i].LanguageId = languageId;
            contentLocalizations[i].TranslationStatus = TranslationStatus.Relevant;
        }

        var contentIds = contentLocalizations.Select(l => l.EntityId).ToList();

        var existingContentLocalizations = (await _repositoryWrapper.ProgramSectionContentLocalizationsRepository
            .GetAllAsync(new QueryOptions<ProgramSectionContentLocalization>
            {
                Filter = l => l.LanguageId == languageId && contentIds.Contains(l.EntityId)
            })).ToList();

        var existingContentLocalizationsById = existingContentLocalizations.ToDictionary(l => l.EntityId);

        var contentLocalizationsToUpdate = contentLocalizations.Where(l => existingContentLocalizationsById.ContainsKey(l.EntityId)).ToList();
        var contentLocalizationsToCreate = contentLocalizations.Where(l => !existingContentLocalizationsById.ContainsKey(l.EntityId)).ToList();

        if (contentLocalizationsToUpdate.Count > 0)
        {
            foreach (var loc in contentLocalizationsToUpdate)
            {
                loc.CreatedAt = existingContentLocalizationsById[loc.EntityId].CreatedAt;
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(contentLocalizationsToUpdate, true);
        }

        if (contentLocalizationsToCreate.Count > 0)
        {
            var utcNow = _timeProvider.GetUtcNow();
            foreach (var loc in contentLocalizationsToCreate)
            {
                loc.CreatedAt = utcNow;
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(contentLocalizationsToCreate, false);
        }
    }
}
