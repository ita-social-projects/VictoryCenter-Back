using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Create;

public class CreateHistoryLocalizationHandler : IRequestHandler<CreateHistoryLocalizationCommand, Result<List<HistorySectionLocalizationDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> _contentLocalizationService;
    private readonly IValidator<CreateHistoryLocalizationCommand> _validator;
    public CreateHistoryLocalizationHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> contentLocalizationService, IValidator<CreateHistoryLocalizationCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _contentLocalizationService = contentLocalizationService;
        _validator = validator;
    }

    public async Task<Result<List<HistorySectionLocalizationDto>>> Handle(CreateHistoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var allSections = (await _repositoryWrapper.HistorySectionsRepository.GetAllAsync()).ToList();

            var requestSectionIds = request.CreateHistorySectionLocalizationDtos
                .Select(x => x.EntityId)
                .ToHashSet();

            var missingSectionIds = allSections
                .Select(s => s.Id)
                .Where(id => !requestSectionIds.Contains(id))
                .ToList();

            if (missingSectionIds.Count > 0)
            {
                return Result.Fail<List<HistorySectionLocalizationDto>>(
                    ErrorMessagesConstants.MissingSectionsLocalization(missingSectionIds));
            }

            var allContentLocalizations = new List<HistorySectionContentLocalization>();
            var sectionById = new Dictionary<long, HistorySection>();

            foreach (var sectionDto in request.CreateHistorySectionLocalizationDtos)
            {
                var section = await _repositoryWrapper.HistorySectionsRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<HistorySection>
                    {
                        Filter = x => x.Id == sectionDto.EntityId,
                        Include = x => x.Include(x => x.Contents)
                    });

                if (section is null)
                {
                    return Result.Fail<List<HistorySectionLocalizationDto>>(ErrorMessagesConstants.NotFound(sectionDto.EntityId, typeof(HistorySection)));
                }

                var validationResult = HistorySectionContentLocalizationValidationHelper.ValidateSectionContents(
                    section.Id,
                    sectionDto.Contents,
                    section.Contents);

                if (validationResult.IsFailed)
                {
                    return Result.Fail<List<HistorySectionLocalizationDto>>(validationResult.Errors);
                }

                var contentLocalizations = _mapper.Map<List<HistorySectionContentLocalization>>(sectionDto.Contents);
                allContentLocalizations.AddRange(contentLocalizations);
                sectionById[section.Id] = section;
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(allContentLocalizations, false);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<List<HistorySectionLocalizationDto>>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(HistorySectionLocalizationDto)));
            }

            var allEntityIds = allContentLocalizations.Select(c => c.EntityId).ToList();
            var allLanguageIds = allContentLocalizations.Select(c => c.LanguageId).ToList();

            var createdLocalizations = await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => allEntityIds.Contains(l.EntityId) &&
                                  allLanguageIds.Contains(l.LanguageId),
                    Include = q => q.Include(l => l.Language)
                });

            var localizationsByContentId = createdLocalizations
                .GroupBy(l => l.EntityId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var results = new List<HistorySectionLocalizationDto>();
            foreach (var (sectionId, section) in sectionById)
            {
                var contentIds = section.Contents.Select(c => c.Id).ToHashSet();
                var sectionLocalizations = createdLocalizations
                    .Where(l => contentIds.Contains(l.EntityId))
                    .ToList();

                results.Add(new HistorySectionLocalizationDto
                {
                    EntityId = sectionId,
                    Contents = _mapper.Map<List<HistorySectionContentLocalizationDto>>(sectionLocalizations)
                });
            }

            return Result.Ok(results);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HistorySectionLocalizationDto)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(HistorySectionLocalizationDto)));
        }
        catch (Exception ex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ex.Message);
        }
    }
}
