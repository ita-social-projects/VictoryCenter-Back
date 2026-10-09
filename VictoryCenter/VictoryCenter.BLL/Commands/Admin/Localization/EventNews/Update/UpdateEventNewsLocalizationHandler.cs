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

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;

public class UpdateEventNewsLocalizationHandler : IRequestHandler<UpdateEventNewsLocalizationCommand, Result<EventNewsLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateEventNewsLocalizationCommand> _validator;
    private readonly ILocalizationService<EventNewsEntity, EventNewsLocalization> _localizationService;

    public UpdateEventNewsLocalizationHandler(
        IMapper mapper,
        IValidator<UpdateEventNewsLocalizationCommand> validator,
        ILocalizationService<EventNewsEntity, EventNewsLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<EventNewsLocalizationDto>> Handle(UpdateEventNewsLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdateEventNewsLocalizationDto;
            EventNewsLocalization entity = _mapper.Map<EventNewsLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            EventNewsLocalizationDto responseDto = _mapper.Map<EventNewsLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<EventNewsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<EventNewsLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<EventNewsLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<EventNewsLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventNewsLocalization)));
        }
    }
}
