using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BulkDelete;

public record BulkDeleteReportFundsExpendituresRecordCommand(IEnumerable<long> Ids)
    : IRequest<Result<long[]>>;
