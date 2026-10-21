using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventsIntroSectionEntity = VictoryCenter.DAL.Entities.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;

public class UpdateEventsIntroSectionLocalizationHandler
    : IRequestHandler<UpdateEventsIntroSectionLocalizationCommand, Result<EventsIntroSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateEventsIntroSectionLocalizationCommand> _validator;

    public UpdateEventsIntroSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateEventsIntroSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<EventsIntroSectionLocalizationDto>> Handle(
        UpdateEventsIntroSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = await _repositoryWrapper.EventsIntroSectionLocalizationsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<EventsIntroSectionLocalization>
                {
                    Filter = localization => localization.EntityId == request.EntityId
                        && localization.LanguageId == request.LanguageId,
                });

            if (entity is null)
            {
                throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(
                    (request.EntityId, request.LanguageId),
                    typeof(EventsIntroSectionLocalization)));
            }

            _mapper.Map(request.UpdateEventsIntroSectionLocalizationDto, entity);
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<EventsIntroSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<EventsIntroSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<EventsIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventsIntroSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<EventsIntroSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<EventsIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventsIntroSectionLocalization)));
        }
    }
}
