using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;

public record PublishReportFundsExpendituresCommand : IRequest<Result<bool>>;
