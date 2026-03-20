using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;

public record UpdateReportFundsExpendituresRecordCommand(
    UpdateReportFundsExpendituresRecordDto UpdateReportFundsExpendituresRecordDto, long Id)
    : IRequest<Result<ReportFundsExpendituresRecordDto>>;
