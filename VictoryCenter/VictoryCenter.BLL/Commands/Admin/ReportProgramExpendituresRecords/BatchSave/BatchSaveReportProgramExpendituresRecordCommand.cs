using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BatchSave;

public record BatchSaveReportProgramExpendituresRecordCommand(
    BatchSaveReportProgramExpendituresRecordsDto BatchSaveReportProgramExpendituresRecordsDto)
    : IRequest<Result<Unit>>;
