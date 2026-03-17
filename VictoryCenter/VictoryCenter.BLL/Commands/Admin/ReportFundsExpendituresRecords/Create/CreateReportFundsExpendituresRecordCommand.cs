using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;

public record CreateReportFundsExpendituresRecordCommand(
    CreateReportFundsExpendituresRecordDto CreateReportFundsExpendituresRecordDto)
    : IRequest<Result<ReportFundsExpendituresRecordDto>>;
