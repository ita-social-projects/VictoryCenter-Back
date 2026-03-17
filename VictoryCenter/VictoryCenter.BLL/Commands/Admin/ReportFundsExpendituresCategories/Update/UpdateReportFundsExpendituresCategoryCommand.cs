using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;

public record UpdateReportFundsExpendituresCategoryCommand(
    UpdateReportFundsExpendituresCategoryDto UpdateReportFundsExpendituresCategoryDto, long Id)
    : IRequest<Result<ReportFundsExpendituresCategoryDto>>;
