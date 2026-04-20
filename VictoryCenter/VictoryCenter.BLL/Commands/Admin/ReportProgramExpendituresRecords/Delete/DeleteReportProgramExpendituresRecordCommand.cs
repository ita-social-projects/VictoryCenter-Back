using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;

public record DeleteReportProgramExpendituresRecordCommand(long ReportProgramExpendituresRecordId)
    : IRequest<Result<long>>;
