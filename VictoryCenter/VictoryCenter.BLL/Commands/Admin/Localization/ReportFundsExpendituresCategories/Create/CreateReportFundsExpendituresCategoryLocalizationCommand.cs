using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Create;

public record CreateReportFundsExpendituresCategoryLocalizationCommand(
    CreateReportFundsExpendituresCategoryLocalizationDto CreateReportFundsExpendituresCategoryLocalizationDto)
    : IRequest<Result<ReportFundsExpendituresCategoryLocalizationDto>>;
