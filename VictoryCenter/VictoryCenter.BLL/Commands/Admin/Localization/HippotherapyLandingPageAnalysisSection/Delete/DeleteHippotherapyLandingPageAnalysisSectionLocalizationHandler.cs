using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnalysisSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Delete;

public class DeleteHippotherapyLandingPageAnalysisSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization> _localizationService;

    public DeleteHippotherapyLandingPageAnalysisSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageAnalysisSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageAnalysisSectionLocalization)));
        }
    }
}
