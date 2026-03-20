using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetAll;

public record GetAllReportFundsExpendituresRecordsQuery
    : IRequest<Result<IEnumerable<ReportFundsExpendituresRecordDto>>>;
