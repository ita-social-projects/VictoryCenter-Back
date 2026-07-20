using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Cancel;

public class CancelReportFundsExpendituresHandler
    : IRequestHandler<CancelReportFundsExpendituresCommand, Result<bool>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    public CancelReportFundsExpendituresHandler(
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider)
    {
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
    }

    public async Task<Result<bool>> Handle(
        CancelReportFundsExpendituresCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var backupSettings = await _repositoryWrapper.BackupReportFundsExpendituresSettingsRepository
                .GetFirstOrDefaultAsync();

            if (backupSettings is null)
            {
                return Result.Fail<bool>("Неможливо відмінити зміни, оскільки не знайдено попередньої опублікованої версії. Будь ласка, спочатку збережіть та опублікуйте звіт.");
            }

            var backupSettingsLocalizations = (await _repositoryWrapper.BackupReportFundsExpendituresSettingsLocalizationsRepository
                .GetAllAsync(new QueryOptions<BackupReportFundsExpendituresSettingsLocalization>
                {
                    Include = q => q.Include(l => l.Language),
                    AsNoTracking = true
                })).ToList();

            var backupCategories = (await _repositoryWrapper.BackupReportFundsExpendituresCategoriesRepository
                .GetAllAsync(new QueryOptions<BackupReportFundsExpendituresCategory>
                {
                    Include = q => q.Include(c => c.Localizations).ThenInclude(l => l.Language),
                    AsNoTracking = true
                })).ToList();

            var backupFundsRecords = (await _repositoryWrapper.BackupReportFundsExpendituresRecordsRepository
                .GetAllAsync(new QueryOptions<BackupReportFundsExpendituresRecord>
                {
                    AsNoTracking = true
                })).ToList();

            var backupProgramRecords = (await _repositoryWrapper.BackupReportProgramExpendituresRecordsRepository
                .GetAllAsync(new QueryOptions<BackupReportProgramExpendituresRecord>
                {
                    AsNoTracking = true
                })).ToList();

            using var transaction = _repositoryWrapper.BeginTransaction();

            await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);

            await _repositoryWrapper.ReportProgramExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);

            await _repositoryWrapper.ReportFundsExpendituresCategoryLocalizationsRepository
                .BulkDeleteAsync(_ => true);

            await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
                .BulkDeleteAsync(_ => true);

            await _repositoryWrapper.ReportFundsExpendituresSettingsLocalizationsRepository
                .BulkDeleteAsync(_ => true);

            var now = _timeProvider.GetUtcNow();

            var oldToNewCategoryMap = new Dictionary<long, ReportFundsExpendituresCategory>();

            var restoredCategories = backupCategories.Select(bc =>
            {
                var newCategory = new ReportFundsExpendituresCategory
                {
                    Name = bc.Name,
                    Type = bc.Type,
                    CreatedAt = bc.CreatedAt,
                };
                oldToNewCategoryMap[bc.Id] = newCategory;
                return newCategory;
            }).ToArray();

            if (restoredCategories.Any())
            {
                await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.CreateRangeAsync(restoredCategories);
                await _repositoryWrapper.SaveChangesAsync();
            }

            var restoredCategoryLocalizations = backupCategories
                .SelectMany(bc => bc.Localizations.Select(bl => new ReportFundsExpendituresCategoryLocalization
                {
                    EntityId = oldToNewCategoryMap[bl.EntityId].Id,
                    LanguageId = bl.LanguageId,
                    Name = bl.Name,
                    TranslationStatus = bl.TranslationStatus,
                    CreatedAt = bl.CreatedAt,
                })).ToArray();

            await _repositoryWrapper.ReportFundsExpendituresCategoryLocalizationsRepository
                .CreateRangeAsync(restoredCategoryLocalizations);

            var restoredFundsRecords = backupFundsRecords.Select(br => new ReportFundsExpendituresRecord
            {
                CategoryId = oldToNewCategoryMap[br.CategoryId].Id,
                Type = br.Type,
                ReportingYear = br.ReportingYear,
                AmountUah = br.AmountUah,
                AmountUsd = br.AmountUsd,
                CreatedAt = br.CreatedAt,
            }).ToArray();

            if (restoredFundsRecords.Any())
            {
                await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.CreateRangeAsync(restoredFundsRecords);
            }

            var restoredProgramRecords = backupProgramRecords.Select(br => new ReportProgramExpendituresRecord
            {
                HippotherapyProgramCategoryId = br.HippotherapyProgramCategoryId,
                ReportingYear = br.ReportingYear,
                AmountUah = br.AmountUah,
                AmountUsd = br.AmountUsd,
                CreatedAt = br.CreatedAt,
            }).ToArray();

            if (restoredProgramRecords.Any())
            {
                await _repositoryWrapper.ReportProgramExpendituresRecordsRepository.CreateRangeAsync(restoredProgramRecords);
            }

            var settingsResult = await ReportFundsExpendituresSettingsHelper
                .GetOrCreateSettingsAsync(_repositoryWrapper, _timeProvider);

            if (settingsResult.IsFailed)
            {
                return Result.Fail<bool>(settingsResult.Errors);
            }

            var settings = settingsResult.Value;
            settings.DisclaimerTitle = backupSettings.DisclaimerTitle;
            settings.ExchangeRate = backupSettings.ExchangeRate;
            settings.ProgramExpendituresReportingYear = backupSettings.ProgramExpendituresReportingYear;
            settings.HasUnpublishedChanges = false;

            _repositoryWrapper.ReportFundsExpendituresSettingsRepository.Update(settings);

            var restoredSettingsLocalizations = backupSettingsLocalizations.Select(bl =>
                new ReportFundsExpendituresSettingsLocalization
                {
                    EntityId = settings.Id,
                    LanguageId = bl.LanguageId,
                    DisclaimerTitle = bl.DisclaimerTitle,
                    TranslationStatus = bl.TranslationStatus,
                    CreatedAt = bl.CreatedAt,
                }).ToArray();

            await _repositoryWrapper.ReportFundsExpendituresSettingsLocalizationsRepository
                .CreateRangeAsync(restoredSettingsLocalizations);

            await _repositoryWrapper.SaveChangesAsync();

            transaction.Complete();

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to cancel report funds expenditures changes: {ex.Message} {ex.InnerException?.Message}");
        }
    }
}
