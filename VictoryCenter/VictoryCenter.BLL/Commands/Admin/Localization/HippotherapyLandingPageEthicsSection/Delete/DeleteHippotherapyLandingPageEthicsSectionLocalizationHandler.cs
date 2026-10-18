using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageEthicsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Delete;

public class DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageEthicsSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
    }
}
