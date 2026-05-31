using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetSummary;

public class GetReportProgramExpendituresSummaryHandler
    : IRequestHandler<GetReportProgramExpendituresSummaryQuery, Result<ReportProgramExpendituresSummaryDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetReportProgramExpendituresSummaryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<ReportProgramExpendituresSummaryDto>> Handle(
        GetReportProgramExpendituresSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var (totalAmountUah, totalAmountUsd) =
            await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.GetSummaryAsync();

        return Result.Ok(new ReportProgramExpendituresSummaryDto
        {
            TotalAmountUah = totalAmountUah,
            TotalAmountUsd = totalAmountUsd,
        });
    }
}
