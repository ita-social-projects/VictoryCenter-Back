using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Delete;

public class DeleteHippotherapyProgramCategoryLocalizationHandler
    : IRequestHandler<DeleteHippotherapyProgramCategoryLocalizationCommand, Result<DeleteHippotherapyProgramCategoryLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyProgramCategory, HippotherapyProgramCategoryLocalization> _localizationService;

    public DeleteHippotherapyProgramCategoryLocalizationHandler(ILocalizationService<HippotherapyProgramCategory, HippotherapyProgramCategoryLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteHippotherapyProgramCategoryLocalizationDto>> Handle(
        DeleteHippotherapyProgramCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteHippotherapyProgramCategoryLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyProgramCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyProgramCategoryLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgramCategoryLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyProgramCategoryLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyProgramCategoryLocalization)));
        }
    }
}
