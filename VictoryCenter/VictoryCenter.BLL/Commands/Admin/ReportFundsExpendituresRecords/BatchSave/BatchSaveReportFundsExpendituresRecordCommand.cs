using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;

public record BatchSaveReportFundsExpendituresRecordCommand(
    BatchSaveReportFundsExpendituresRecordsDto BatchSaveReportFundsExpendituresRecordsDto)
    : IRequest<Result<Unit>>;
