using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Delete;

public class DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
    }
}
