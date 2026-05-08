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

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

public class UpdateHistoryLocalizationHandler : IRequestHandler<UpdateHistoryLocalizationCommand, Result<HistorySectionLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> _contentLocalizationService;

    public UpdateHistoryLocalizationHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILocalizationService<HistorySectionContent, HistorySectionContentLocalization> contentLocalizationService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _contentLocalizationService = contentLocalizationService;
    }

    public async Task<Result<HistorySectionLocalizationDto>> Handle(UpdateHistoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var section = await _repositoryWrapper.HistorySectionsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<HistorySection>
                {
                    Filter = x => x.Id == request.EntityId,
                    Include = x => x.Include(s => s.Contents)
                });

            if (section is null)
            {
                return Result.Fail<HistorySectionLocalizationDto>(ErrorMessagesConstants.NotFound(request.EntityId, typeof(HistorySection)));
            }

            var contentTypesById = section.Contents.ToDictionary(c => c.Id, c => c.ContentType);

            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(
                request.UpdateHistorySectionLocalizationDto.Contents,
                contentTypesById);

            var contentLocalizations = _mapper.Map<List<HistorySectionContentLocalization>>(request.UpdateHistorySectionLocalizationDto.Contents);

            for (int i = 0; i < contentLocalizations.Count; i++)
            {
                contentLocalizations[i].EntityId = request.UpdateHistorySectionLocalizationDto.Contents[i].EntityId;
                contentLocalizations[i].LanguageId = request.LanguageId;
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(contentLocalizations, true);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<HistorySectionLocalizationDto>(
                    ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
            }

            var updatedLocalizations = await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => l.LanguageId == request.LanguageId &&
                                  section.Contents.Select(c => c.Id).Contains(l.EntityId),
                    Include = q => q.Include(l => l.Language)
                });

            var result = new HistorySectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = _mapper.Map<List<HistorySectionContentLocalizationDto>>(updatedLocalizations)
            };

            return Result.Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail<HistorySectionLocalizationDto>(ex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HistorySectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySectionContentLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HistorySectionLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HistorySectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
        }
    }
}
