using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionCenterSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Delete;

public class DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)));
        }
    }
}
