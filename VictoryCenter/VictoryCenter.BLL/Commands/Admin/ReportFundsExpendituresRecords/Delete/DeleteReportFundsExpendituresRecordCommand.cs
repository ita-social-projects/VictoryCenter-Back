using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Delete;

public record DeleteReportFundsExpendituresRecordCommand(long Id) : IRequest<Result<long>>;
