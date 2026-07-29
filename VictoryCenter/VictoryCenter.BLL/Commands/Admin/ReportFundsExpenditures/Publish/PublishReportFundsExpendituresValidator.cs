using FluentValidation;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;

public class PublishReportFundsExpendituresValidator
    : AbstractValidator<PublishReportFundsExpendituresCommand>
{
    private const int MinExpenseRecords = 2;
    private const int MinIncomeRecords = 2;
    private const int MinProgramExpenditureRecords = 1;

    public PublishReportFundsExpendituresValidator(IRepositoryWrapper repositoryWrapper)
    {
        RuleFor(command => command)
            .MustAsync(async (_, cancellationToken) =>
            {
                var expenseCount = await repositoryWrapper.ReportFundsExpendituresRecordsRepository
                    .CountAsync(new QueryOptions<ReportFundsExpendituresRecord>
                    {
                        Filter = record => record.Type == ReportFundsExpendituresType.Expense
                    });
                return expenseCount >= MinExpenseRecords;
            })
            .WithMessage($"At least {MinExpenseRecords} expense records are required to publish.");

        RuleFor(command => command)
            .MustAsync(async (_, cancellationToken) =>
            {
                var incomeCount = await repositoryWrapper.ReportFundsExpendituresRecordsRepository
                    .CountAsync(new QueryOptions<ReportFundsExpendituresRecord>
                    {
                        Filter = record => record.Type == ReportFundsExpendituresType.Income
                    });
                return incomeCount >= MinIncomeRecords;
            })
            .WithMessage($"At least {MinIncomeRecords} fund records are required to publish.");

        RuleFor(command => command)
            .MustAsync(async (_, cancellationToken) =>
            {
                var programCount = await repositoryWrapper.ReportProgramExpendituresRecordsRepository
                    .CountAsync();
                return programCount >= MinProgramExpenditureRecords;
            })
            .WithMessage($"At least {MinProgramExpenditureRecords} program expenditure record is required to publish.");
    }
}
