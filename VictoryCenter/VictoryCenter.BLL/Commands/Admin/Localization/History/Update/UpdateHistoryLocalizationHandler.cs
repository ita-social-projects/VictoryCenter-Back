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

                HistorySectionContentLocalizationValidationHelper.ValidateSectionContents(
                    section.Id,
                    sectionDto.Contents,
                    section.Contents);

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

            var localizationsToUpdate = allContentLocalizations.Where(l => existingContentIds.Contains(l.EntityId)).ToList();
            var localizationsToCreate = allContentLocalizations.Where(l => !existingContentIds.Contains(l.EntityId)).ToList();

            if (localizationsToUpdate.Count > 0)
            {
                foreach (var loc in localizationsToUpdate)
                {
                    loc.TranslationStatus = TranslationStatus.Relevant;
                }

                await _contentLocalizationService.TrackEntityLocalizationAsync(localizationsToUpdate, true);
            }

            if (localizationsToCreate.Count > 0)
            {
                var utcNow = DateTimeOffset.UtcNow;
                foreach (var loc in localizationsToCreate)
                {
                    loc.CreatedAt = utcNow;
                    loc.TranslationStatus = TranslationStatus.Relevant;
                }

                await _contentLocalizationService.TrackEntityLocalizationAsync(localizationsToCreate, false);
            }

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
        catch (Exception)
        {
            return Result.Fail<List<HistorySectionLocalizationDto>>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySectionContentLocalization)));
        }
    }
}
