using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Delete;

public class DeleteReportFundsExpendituresCategoryLocalizationHandler
    : IRequestHandler<DeleteReportFundsExpendituresCategoryLocalizationCommand, Result<DeleteReportFundsExpendituresCategoryLocalizationDto>>
{
    private readonly ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> _localizationService;

    public DeleteReportFundsExpendituresCategoryLocalizationHandler(
        ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteReportFundsExpendituresCategoryLocalizationDto>> Handle(
        DeleteReportFundsExpendituresCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteReportFundsExpendituresCategoryLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteReportFundsExpendituresCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteReportFundsExpendituresCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteReportFundsExpendituresCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
    }
}
