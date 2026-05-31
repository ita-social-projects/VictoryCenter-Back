using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;

public record UpdateReportProgramExpendituresRecordCommand(
    long ReportProgramExpendituresRecordId,
    UpdateReportProgramExpendituresRecordDto Dto)
    : IRequest<Result<ReportProgramExpendituresRecordDto>>;
