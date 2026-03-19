using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetSummary;

public class GetReportFundsExpendituresSummaryHandler
    : IRequestHandler<GetReportFundsExpendituresSummaryQuery, Result<ReportFundsExpendituresSummaryDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetReportFundsExpendituresSummaryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<ReportFundsExpendituresSummaryDto>> Handle(
        GetReportFundsExpendituresSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var (incomeUahTotal, incomeUsdTotal, incomeCategoriesCount, expenditureUahTotal, expenditureUsdTotal,
            expenditureCategoriesCount) =
            await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.GetSummaryAsync();

        return Result.Ok(new ReportFundsExpendituresSummaryDto
        {
            IncomeUahTotal = incomeUahTotal,
            IncomeUsdTotal = incomeUsdTotal,
            ExpenditureUahTotal = expenditureUahTotal,
            ExpenditureUsdTotal = expenditureUsdTotal,
            IncomeCategoriesCount = incomeCategoriesCount,
            ExpenditureCategoriesCount = expenditureCategoriesCount,
        });
    }
}
