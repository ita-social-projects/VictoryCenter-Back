using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;

public class CreateEventNewsLocalizationHandler : IRequestHandler<CreateEventNewsLocalizationCommand, Result<EventNewsLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateEventNewsLocalizationCommand> _validator;
    private readonly ILocalizationService<EventNewsEntity, EventNewsLocalization> _localizationService;

    public CreateEventNewsLocalizationHandler(
        IMapper mapper,
        IValidator<CreateEventNewsLocalizationCommand> validator,
        ILocalizationService<EventNewsEntity, EventNewsLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<EventNewsLocalizationDto>> Handle(CreateEventNewsLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            EventNewsLocalization entity = _mapper.Map<EventNewsLocalization>(request.CreateEventNewsLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            EventNewsLocalizationDto responseDto = _mapper.Map<EventNewsLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<EventNewsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<EventNewsLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<EventNewsLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<EventNewsLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(EventNewsLocalization)));
        }
    }
}
