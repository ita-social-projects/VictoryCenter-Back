using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventsIntroSectionEntity = VictoryCenter.DAL.Entities.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;

public class UpdateEventsIntroSectionLocalizationHandler
    : IRequestHandler<UpdateEventsIntroSectionLocalizationCommand, Result<EventsIntroSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> _localizationService;
    private readonly IValidator<UpdateEventsIntroSectionLocalizationCommand> _validator;

    public UpdateEventsIntroSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> localizationService,
        IValidator<UpdateEventsIntroSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<EventsIntroSectionLocalizationDto>> Handle(
        UpdateEventsIntroSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<EventsIntroSectionLocalization>(
                request.UpdateEventsIntroSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
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
