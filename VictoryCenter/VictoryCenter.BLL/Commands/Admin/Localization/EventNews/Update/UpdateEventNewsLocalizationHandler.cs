using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;

public class UpdateEventNewsLocalizationHandler : IRequestHandler<UpdateEventNewsLocalizationCommand, Result<AdminEventNewsLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<EventNewsEntity, EventNewsLocalization> _localizationService;

    public UpdateEventNewsLocalizationHandler(
        IMapper mapper,
        ILocalizationService<EventNewsEntity, EventNewsLocalization> localizationService)
    {
        _mapper = mapper;
        _localizationService = localizationService;
    }

    public async Task<Result<AdminEventNewsLocalizationDto>> Handle(UpdateEventNewsLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.UpdateEventNewsLocalizationDto;
            EventNewsLocalization entity = _mapper.Map<EventNewsLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            AdminEventNewsLocalizationDto responseDto = _mapper.Map<AdminEventNewsLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventNewsLocalization)));
        }
    }
}
