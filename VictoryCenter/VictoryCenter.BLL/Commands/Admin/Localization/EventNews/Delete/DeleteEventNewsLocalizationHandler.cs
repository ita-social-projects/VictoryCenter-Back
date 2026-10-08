using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;

public class DeleteEventNewsLocalizationHandler : IRequestHandler<DeleteEventNewsLocalizationCommand, Result<DeleteEventNewsLocalizationDto>>
{
    private readonly ILocalizationService<EventNewsEntity, EventNewsLocalization> _localizationService;

    public DeleteEventNewsLocalizationHandler(ILocalizationService<EventNewsEntity, EventNewsLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteEventNewsLocalizationDto>> Handle(DeleteEventNewsLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteEventNewsLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteEventNewsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteEventNewsLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(EventNewsLocalization)));
        }
    }
}
