using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Update;

public record UpdateReportFundsExpendituresCategoryLocalizationCommand(
    UpdateReportFundsExpendituresCategoryLocalizationDto UpdateReportFundsExpendituresCategoryLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<ReportFundsExpendituresCategoryLocalizationDto>>;
