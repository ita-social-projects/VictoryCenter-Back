using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventsIntroSectionEntity = VictoryCenter.DAL.Entities.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Delete;

public class DeleteEventsIntroSectionLocalizationHandler
    : IRequestHandler<DeleteEventsIntroSectionLocalizationCommand, Result<DeleteEventsIntroSectionLocalizationDto>>
{
    private readonly ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> _localizationService;

    public DeleteEventsIntroSectionLocalizationHandler(
        ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteEventsIntroSectionLocalizationDto>> Handle(
        DeleteEventsIntroSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteEventsIntroSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteEventsIntroSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteEventsIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventsIntroSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteEventsIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(EventsIntroSectionLocalization)));
        }
    }
}
