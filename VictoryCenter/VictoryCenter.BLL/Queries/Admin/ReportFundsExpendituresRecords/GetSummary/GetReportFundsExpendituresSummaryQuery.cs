using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetSummary;

public record GetReportFundsExpendituresSummaryQuery
    : IRequest<Result<ReportFundsExpendituresSummaryDto>>;
