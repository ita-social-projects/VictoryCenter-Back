using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Delete;

public record DeleteReportFundsExpendituresCategoryLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteReportFundsExpendituresCategoryLocalizationDto>>;
