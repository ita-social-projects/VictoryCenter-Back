using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageIntroSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Delete;

public class DeleteHippotherapyLandingPageIntroSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageIntroSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageIntroSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageIntroSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageIntroSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageIntroSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageIntroSectionLocalization)));
        }
    }
}
