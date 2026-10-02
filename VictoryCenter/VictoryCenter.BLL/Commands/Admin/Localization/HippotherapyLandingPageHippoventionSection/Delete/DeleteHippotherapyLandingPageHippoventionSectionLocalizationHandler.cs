using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionSection.Delete;

public class DeleteHippotherapyLandingPageHippoventionSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageHippoventionSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageHippoventionSectionEntity, HippotherapyLandingPageHippoventionSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageHippoventionSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageHippoventionSectionEntity, HippotherapyLandingPageHippoventionSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageHippoventionSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageHippoventionSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageHippoventionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageHippoventionSectionLocalization)));
        }
    }
}
