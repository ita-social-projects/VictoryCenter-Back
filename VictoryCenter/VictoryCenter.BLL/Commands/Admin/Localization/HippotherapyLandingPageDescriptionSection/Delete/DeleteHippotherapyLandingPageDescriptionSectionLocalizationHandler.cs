using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageDescriptionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Delete;

public class DeleteHippotherapyLandingPageDescriptionSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageDescriptionSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
    }
}
