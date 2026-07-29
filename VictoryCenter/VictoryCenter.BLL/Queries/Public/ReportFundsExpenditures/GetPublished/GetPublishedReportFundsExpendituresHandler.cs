using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.ReportFundsExpenditures.GetPublished;
using VictoryCenter.BLL.Interfaces.BlobStorage;

public class GetPublishedReportFundsExpendituresHandler
    : IRequestHandler<GetPublishedReportFundsExpendituresQuery, Result<PublishedReportFundsExpendituresDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;

    public GetPublishedReportFundsExpendituresHandler(IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
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

        var collectedFundsBlock = await _repositoryWrapper.GetRepository<CollectedFundsBlock>()
            .GetFirstOrDefaultAsync(new QueryOptions<CollectedFundsBlock>
            {
                Include = q => q.Include(c => c.Image),
                AsNoTracking = true
            });

        var changedLivesBlock = await _repositoryWrapper.GetRepository<ChangedLivesBlock>()
            .GetFirstOrDefaultAsync(new QueryOptions<ChangedLivesBlock>
            {
                Include = q => q.Include(c => c.Image),
                AsNoTracking = true
            });

        var isEnglish = await IsEnglishLanguage(request.LanguageId);

        var mediaSettings = new PublishedReportMediaSettingsDto
        {
            CollectedFunds = new PublishedMediaBlockDto
            {
                Title = Resolve(isEnglish, collectedFundsBlock?.TitleEn, collectedFundsBlock?.Title ?? ""),
                ImageUrl = collectedFundsBlock?.Image != null ? _blobService.GetFileUrl(collectedFundsBlock.Image.BlobName, collectedFundsBlock.Image.MimeType) : null
            },
            ChangedLives = new PublishedMediaBlockDto
            {
                Title = Resolve(isEnglish, changedLivesBlock?.TitleEn, changedLivesBlock?.Title ?? ""),
                ImageUrl = changedLivesBlock?.Image != null ? _blobService.GetFileUrl(changedLivesBlock.Image.BlobName, changedLivesBlock.Image.MimeType) : null,
                Value = changedLivesBlock?.ChangedLivesCount
            }
        };

        if (snapshot is null)
        {
            return Result.Ok(new PublishedReportFundsExpendituresDto
            {
                Settings = new PublishedReportSettingsDto(),
                Funding = new PublishedFundsExpendituresGroupDto(),
                Expenses = new PublishedFundsExpendituresGroupDto(),
                Programs = new PublishedProgramExpendituresGroupDto(),
                MediaSettings = mediaSettings
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
            Programs = programs,
            MediaSettings = mediaSettings
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
