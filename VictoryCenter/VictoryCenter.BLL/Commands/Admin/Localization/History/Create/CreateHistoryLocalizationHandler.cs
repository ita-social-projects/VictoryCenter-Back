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

public class CreateHistoryLocalizationHandler : IRequestHandler<CreateHistoryLocalizationCommand, Result<HistorySectionLocalizationDto>>
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

    public async Task<Result<HistorySectionLocalizationDto>> Handle(CreateHistoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var section = await _repositoryWrapper.HistorySectionsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<HistorySection>
                {
                    Filter = x => x.Id == request.CreateHistorySectionLocalizationDto.EntityId,
                    Include = x => x.Include(x => x.Contents)
                });
            if (section is null)
            {
                return Result.Fail<HistorySectionLocalizationDto>(ErrorMessagesConstants.NotFound(request.CreateHistorySectionLocalizationDto.EntityId, typeof(HistorySection)));
            }

            var contentTypesById = section.Contents.ToDictionary(c => c.Id, c => c.ContentType);

            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(
                request.CreateHistorySectionLocalizationDto.Contents,
                contentTypesById);

            var contentDtos = request.CreateHistorySectionLocalizationDto.Contents.ToList();
            var contentLocalizations = _mapper.Map<List<HistorySectionContentLocalization>>(contentDtos);
            await _contentLocalizationService.TrackEntityLocalizationAsync(contentLocalizations, false);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<HistorySectionLocalizationDto>("Failed to save in Database");
            }

            var createdLocalizations = await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => contentLocalizations.Select(c => c.EntityId).Contains(l.EntityId) &&
                                  contentLocalizations.Select(c => c.LanguageId).Contains(l.LanguageId),
                    Include = q => q.Include(l => l.Language)
                });

            var result = new HistorySectionLocalizationDto
            {
                EntityId = section.Id,
                Contents = _mapper.Map<List<HistorySectionContentLocalizationDto>>(contentLocalizations)
            };

            return Result.Ok(result);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HistorySectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HistorySectionLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HistorySectionLocalizationDto)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HistorySectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HistorySectionLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(HistorySectionLocalizationDto)));
        }
        catch (Exception ex)
        {
            return Result.Fail<HistorySectionLocalizationDto>(ex.Message);
        }
    }
}
