using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.ReportFundsExpenditures.GetPublished;

public class GetPublishedReportFundsExpendituresHandler
    : IRequestHandler<GetPublishedReportFundsExpendituresQuery, Result<PublishedReportFundsExpendituresDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedReportFundsExpendituresHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PublishedReportFundsExpendituresDto>> Handle(
        GetPublishedReportFundsExpendituresQuery request,
        CancellationToken cancellationToken)
    {
        var snapshot = await _repositoryWrapper.PublishedReportFundsExpendituresSnapshotRepository
            .GetFirstOrDefaultAsync(new QueryOptions<PublishedReportFundsExpendituresSnapshot>
            {
                AsNoTracking = true
            });

        if (snapshot is null)
        {
            return Result.Ok(new PublishedReportFundsExpendituresDto
            {
                Settings = new PublishedReportSettingsDto(),
                Funding = new PublishedFundsExpendituresGroupDto(),
                Expenses = new PublishedFundsExpendituresGroupDto(),
                Programs = new PublishedProgramExpendituresGroupDto()
            });
        }

        var fundsRecords = (await _repositoryWrapper.PublishedReportFundsExpendituresRecordsRepository
            .GetAllAsync(new QueryOptions<PublishedReportFundsExpendituresRecord>
            {
                AsNoTracking = true
            })).ToList();

        var programRecords = (await _repositoryWrapper.PublishedReportProgramExpendituresRecordsRepository
            .GetAllAsync(new QueryOptions<PublishedReportProgramExpendituresRecord>
            {
                AsNoTracking = true
            })).ToList();

        var isEnglish = await IsEnglishLanguage(request.LanguageId);

        var settings = new PublishedReportSettingsDto
        {
            DisclaimerTitle = Resolve(isEnglish, snapshot.DisclaimerTitleEn, snapshot.DisclaimerTitle),
            ExchangeRate = snapshot.ExchangeRate,
            ProgramExpendituresReportingYear = snapshot.ProgramExpendituresReportingYear,
            PublishedAt = snapshot.PublishedAt
        };

        var incomeRecords = fundsRecords
            .Where(r => r.Type == ReportFundsExpendituresType.Income)
            .ToList();

        var expenseRecords = fundsRecords
            .Where(r => r.Type == ReportFundsExpendituresType.Expense)
            .ToList();

        var funding = new PublishedFundsExpendituresGroupDto
        {
            TotalUah = incomeRecords.Sum(r => r.AmountUah),
            TotalUsd = incomeRecords.Sum(r => r.AmountUsd),
            Items = incomeRecords.Select(r => new PublishedFundsExpendituresItemDto
            {
                Label = Resolve(isEnglish, r.CategoryNameEn, r.CategoryName),
                AmountUah = r.AmountUah,
                AmountUsd = r.AmountUsd
            }).ToList()
        };

        var expenses = new PublishedFundsExpendituresGroupDto
        {
            TotalUah = expenseRecords.Sum(r => r.AmountUah),
            TotalUsd = expenseRecords.Sum(r => r.AmountUsd),
            Items = expenseRecords.Select(r => new PublishedFundsExpendituresItemDto
            {
                Label = Resolve(isEnglish, r.CategoryNameEn, r.CategoryName),
                AmountUah = r.AmountUah,
                AmountUsd = r.AmountUsd
            }).ToList()
        };

        var programs = new PublishedProgramExpendituresGroupDto
        {
            TotalUah = programRecords.Sum(r => r.AmountUah),
            TotalUsd = programRecords.Sum(r => r.AmountUsd),
            Items = programRecords.Select(r => new PublishedProgramExpendituresItemDto
            {
                Label = Resolve(isEnglish, r.CategoryNameEn, r.CategoryName),
                ReportingYear = r.ReportingYear,
                AmountUah = r.AmountUah,
                AmountUsd = r.AmountUsd
            }).ToList()
        };

        return Result.Ok(new PublishedReportFundsExpendituresDto
        {
            Settings = settings,
            Funding = funding,
            Expenses = expenses,
            Programs = programs
        });
    }

    private async Task<bool> IsEnglishLanguage(long? languageId)
    {
        if (!languageId.HasValue)
        {
            return false;
        }

        var language = await _repositoryWrapper.LocalizationLanguagesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<LocalizationLanguage>
            {
                Filter = l => l.Id == languageId.Value,
                AsNoTracking = true
            });

        return language?.Code == "en";
    }

    private static string Resolve(bool isEnglish, string? englishValue, string defaultValue)
    {
        return isEnglish && !string.IsNullOrWhiteSpace(englishValue)
            ? englishValue
            : defaultValue;
    }
}
