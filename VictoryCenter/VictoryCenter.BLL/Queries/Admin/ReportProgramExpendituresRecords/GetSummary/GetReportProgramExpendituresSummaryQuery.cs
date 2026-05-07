using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetSummary;

public record GetReportProgramExpendituresSummaryQuery
    : IRequest<Result<ReportProgramExpendituresSummaryDto>>;
