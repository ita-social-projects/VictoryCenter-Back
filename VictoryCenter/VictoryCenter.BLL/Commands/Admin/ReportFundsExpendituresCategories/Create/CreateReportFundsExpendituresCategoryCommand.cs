using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;

public record CreateReportFundsExpendituresCategoryCommand(
    CreateReportFundsExpendituresCategoryDto CreateReportFundsExpendituresCategoryDto)
    : IRequest<Result<ReportFundsExpendituresCategoryDto>>;
