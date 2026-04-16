using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

public record CreateReportProgramExpendituresRecordCommand(
    CreateReportProgramExpendituresRecordDto CreateReportProgramExpendituresRecordDto)
    : IRequest<Result<ReportProgramExpendituresRecordDto>>;
