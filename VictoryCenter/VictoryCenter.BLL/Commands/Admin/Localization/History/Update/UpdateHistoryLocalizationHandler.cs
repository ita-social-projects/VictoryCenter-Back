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
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

public class UpdateHistoryLocalizationHandler : IRequestHandler<UpdateHistoryLocalizationCommand, Result<List<HistorySectionLocalizationDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> _contentLocalizationService;
    private readonly IValidator<UpdateHistoryLocalizationCommand> _validator;

    public UpdateHistoryLocalizationHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> contentLocalizationService,
        IValidator<UpdateHistoryLocalizationCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _contentLocalizationService = contentLocalizationService;
        _validator = validator;
    }

    public async Task<Result<List<HistorySectionLocalizationDto>>> Handle(UpdateHistoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var allSections = (await _repositoryWrapper.HistorySectionsRepository.GetAllAsync()).ToList();

            var requestSectionIds = request.UpdateHistorySectionLocalizationDtos
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

            foreach (var sectionDto in request.UpdateHistorySectionLocalizationDtos)
            {
                var section = await _repositoryWrapper.HistorySectionsRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<HistorySection>
                    {
                        Filter = x => x.Id == sectionDto.EntityId,
                        Include = x => x.Include(s => s.Contents)
                    });

                if (section is null)
                {
                    return Result.Fail<List<HistorySectionLocalizationDto>>(
                        ErrorMessagesConstants.NotFound(sectionDto.EntityId, typeof(HistorySection)));
                }

                var requestContentIds = sectionDto.Contents.Select(c => c.EntityId).ToHashSet();
                var missingContentIds = section.Contents
                    .Where(c => c.ContentType != ContentType.Image)
                    .Select(c => c.Id)
                    .Where(id => !requestContentIds.Contains(id))
                    .ToList();

                if (missingContentIds.Count > 0)
                {
                    return Result.Fail<List<HistorySectionLocalizationDto>>(
                        ErrorMessagesConstants.MissingContentsLocalization(section.Id, missingContentIds));
                }

                var contentTypesById = section.Contents.ToDictionary(c => c.Id, c => c.ContentType);

                HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(
                    sectionDto.Contents,
                    contentTypesById);

                var contentLocalizations = _mapper.Map<List<HistorySectionContentLocalization>>(sectionDto.Contents);

                for (int i = 0; i < contentLocalizations.Count; i++)
                {
                    contentLocalizations[i].EntityId = sectionDto.Contents[i].EntityId;
                    contentLocalizations[i].LanguageId = request.LanguageId;
                }

                allContentLocalizations.AddRange(contentLocalizations);
                sectionById[section.Id] = section;
            }

            var allContentIds = sectionById.Values
                .SelectMany(s => s.Contents
                    .Where(c => c.ContentType != ContentType.Image)
                    .Select(c => c.Id))
                .ToList();

            var existingLocalizations = (await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => l.LanguageId == request.LanguageId &&
                                  allContentIds.Contains(l.EntityId)
                })).ToList();

            var existingContentIds = existingLocalizations.Select(l => l.EntityId).ToHashSet();
            var notFoundContentIds = allContentIds
                .Where(id => !existingContentIds.Contains(id))
                .ToList();

            if (notFoundContentIds.Count > 0)
            {
                return Result.Fail<List<HistorySectionLocalizationDto>>(
                    ErrorMessagesConstants.NotFound(notFoundContentIds, typeof(HistorySectionContentLocalization)));
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(allContentLocalizations, true);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<List<HistorySectionLocalizationDto>>(
                    ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
            }

            var updatedLocalizations = await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => l.LanguageId == request.LanguageId &&
                                  allContentIds.Contains(l.EntityId),
                    Include = q => q.Include(l => l.Language)
                });

            var results = new List<HistorySectionLocalizationDto>();
            foreach (var (sectionId, section) in sectionById)
            {
                var contentIds = section.Contents.Select(c => c.Id).ToHashSet();
                var sectionLocalizations = updatedLocalizations
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
        catch (KeyNotFoundException ex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySectionContentLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
        }
        catch (Exception ex)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(ex.Message);
        }
    }
}