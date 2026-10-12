using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;

public class CreateEventNewsLocalizationHandler : IRequestHandler<CreateEventNewsLocalizationCommand, Result<AdminEventNewsLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<EventNewsEntity, EventNewsLocalization> _localizationService;

    public CreateEventNewsLocalizationHandler(
        IMapper mapper,
        ILocalizationService<EventNewsEntity, EventNewsLocalization> localizationService)
    {
        _mapper = mapper;
        _localizationService = localizationService;
    }

    public async Task<Result<AdminEventNewsLocalizationDto>> Handle(CreateEventNewsLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            EventNewsLocalization entity = _mapper.Map<EventNewsLocalization>(request.CreateAdminEventNewsLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            AdminEventNewsLocalizationDto responseDto = _mapper.Map<AdminEventNewsLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<AdminEventNewsLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(EventNewsLocalization)));
        }
    }
}
