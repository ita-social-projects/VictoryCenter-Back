using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Delete;

public class DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
    }
}
