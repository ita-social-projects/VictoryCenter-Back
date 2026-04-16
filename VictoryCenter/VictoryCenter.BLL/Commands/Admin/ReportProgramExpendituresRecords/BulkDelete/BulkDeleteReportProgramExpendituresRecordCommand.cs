using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;

public record BulkDeleteReportProgramExpendituresRecordCommand(IEnumerable<long> Ids)
    : IRequest<Result<long[]>>;
