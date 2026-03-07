using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresCategories.GetAll;

public record GetAllReportFundsExpendituresCategoriesQuery
    : IRequest<Result<IEnumerable<ReportFundsExpendituresCategoryDto>>>;
