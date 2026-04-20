using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

public record CreateReportProgramExpendituresRecordCommand(
    CreateReportProgramExpendituresRecordDto CreateReportProgramExpendituresRecordDto)
    : IValidatableRequest<Result<ReportProgramExpendituresRecordDto>>;
