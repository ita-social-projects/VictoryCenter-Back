using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Delete;

public record DeleteReportFundsExpendituresCategoryCommand(long Id) : IRequest<Result<long>>;
