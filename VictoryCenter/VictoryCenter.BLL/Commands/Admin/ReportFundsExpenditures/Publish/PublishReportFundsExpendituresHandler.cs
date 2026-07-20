using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;

public class PublishReportFundsExpendituresHandler
    : IRequestHandler<PublishReportFundsExpendituresCommand, Result<bool>>
{
    private const string EnglishLanguageCode = "en";

    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<PublishReportFundsExpendituresCommand> _validator;
    private readonly TimeProvider _timeProvider;

    public PublishReportFundsExpendituresHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<PublishReportFundsExpendituresCommand> validator,
        TimeProvider timeProvider)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task<Result<bool>> Handle(
        PublishReportFundsExpendituresCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var fundsRecords = (await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
                .GetAllAsync(new QueryOptions<ReportFundsExpendituresRecord>
                {
                    Include = query => query
                        .Include(record => record.Category)
                        .ThenInclude(category => category.Localizations)
                        .ThenInclude(localization => localization.Language),
                    AsNoTracking = true
                })).ToList();

            var programRecords = (await _repositoryWrapper.ReportProgramExpendituresRecordsRepository
                .GetAllAsync(new QueryOptions<ReportProgramExpendituresRecord>
                {
                    Include = query => query
                        .Include(record => record.HippotherapyProgramCategory),
                    AsNoTracking = true
                })).ToList();

            var settingsResult = await ReportFundsExpendituresSettingsHelper
                .GetOrCreateSettingsAsync(_repositoryWrapper, _timeProvider);

            if (settingsResult.IsFailed)
            {
                return Result.Fail<bool>(settingsResult.Errors);
            }

            var settings = settingsResult.Value;

            var settingsLocalizations = (await _repositoryWrapper.ReportFundsExpendituresSettingsLocalizationsRepository
                .GetAllAsync(new QueryOptions<ReportFundsExpendituresSettingsLocalization>
                {
                    Filter = localization => localization.EntityId == settings.Id,
                    Include = query => query.Include(localization => localization.Language),
                    AsNoTracking = true
                })).ToList();

            using var transaction = _repositoryWrapper.BeginTransaction();

            await _repositoryWrapper.PublishedReportFundsExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.PublishedReportProgramExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.PublishedReportFundsExpendituresSnapshotRepository
                .BulkDeleteAsync(_ => true);

            var now = _timeProvider.GetUtcNow();

            var publishedFundsRecords = fundsRecords.Select(record => new PublishedReportFundsExpendituresRecord
            {
                SourceRecordId = record.Id,
                CategoryName = record.Category.Name,
                CategoryNameEn = FindEnglishLocalizationName(record.Category.Localizations),
                Type = record.Type,
                ReportingYear = record.ReportingYear,
                AmountUah = record.AmountUah,
                AmountUsd = record.AmountUsd,
                CreatedAt = now
            }).ToArray();

            var publishedProgramRecords = programRecords.Select(record => new PublishedReportProgramExpendituresRecord
            {
                SourceRecordId = record.Id,
                CategoryName = record.HippotherapyProgramCategory.Name,
                CategoryNameEn = null,
                ReportingYear = record.ReportingYear,
                AmountUah = record.AmountUah,
                AmountUsd = record.AmountUsd,
                CreatedAt = now
            }).ToArray();

            var publishedSnapshot = new PublishedReportFundsExpendituresSnapshot
            {
                DisclaimerTitle = settings.DisclaimerTitle,
                DisclaimerTitleEn = FindEnglishDisclaimerTitle(settingsLocalizations),
                ExchangeRate = settings.ExchangeRate,
                ProgramExpendituresReportingYear = settings.ProgramExpendituresReportingYear,
                PublishedAt = now,
                CreatedAt = now
            };

            await _repositoryWrapper.PublishedReportFundsExpendituresRecordsRepository
                .CreateRangeAsync(publishedFundsRecords);
            await _repositoryWrapper.PublishedReportProgramExpendituresRecordsRepository
                .CreateRangeAsync(publishedProgramRecords);
            await _repositoryWrapper.PublishedReportFundsExpendituresSnapshotRepository
                .CreateAsync(publishedSnapshot);

            settings.HasUnpublishedChanges = false;
            _repositoryWrapper.ReportFundsExpendituresSettingsRepository.Update(settings);

            // --- Snapshot draft state into backup tables for Cancel support ---

            await _repositoryWrapper.BackupReportFundsExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.BackupReportProgramExpendituresRecordsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.BackupReportFundsExpendituresCategoryLocalizationsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.BackupReportFundsExpendituresCategoriesRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.BackupReportFundsExpendituresSettingsLocalizationsRepository
                .BulkDeleteAsync(_ => true);
            await _repositoryWrapper.BackupReportFundsExpendituresSettingsRepository
                .BulkDeleteAsync(_ => true);

            var backupSettings = new BackupReportFundsExpendituresSettings
            {
                Id = settings.Id,
                DisclaimerTitle = settings.DisclaimerTitle,
                ExchangeRate = settings.ExchangeRate,
                ProgramExpendituresReportingYear = settings.ProgramExpendituresReportingYear,
                CreatedAt = settings.CreatedAt,
            };
            await _repositoryWrapper.BackupReportFundsExpendituresSettingsRepository
                .CreateAsync(backupSettings);

            var backupSettingsLocalizations = settingsLocalizations.Select(sl =>
                new BackupReportFundsExpendituresSettingsLocalization
                {
                    EntityId = sl.EntityId,
                    LanguageId = sl.LanguageId,
                    DisclaimerTitle = sl.DisclaimerTitle,
                    TranslationStatus = sl.TranslationStatus,
                    CreatedAt = sl.CreatedAt,
                }).ToArray();
            await _repositoryWrapper.BackupReportFundsExpendituresSettingsLocalizationsRepository
                .CreateRangeAsync(backupSettingsLocalizations);

            var backupCategories = fundsRecords
                .Select(r => r.Category)
                .DistinctBy(c => c.Id)
                .Select(c => new BackupReportFundsExpendituresCategory
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    CreatedAt = c.CreatedAt,
                }).ToArray();
            await _repositoryWrapper.BackupReportFundsExpendituresCategoriesRepository
                .CreateRangeAsync(backupCategories);

            var backupCategoryLocalizations = fundsRecords
                .Select(r => r.Category)
                .DistinctBy(c => c.Id)
                .SelectMany(c => c.Localizations.Select(l => new BackupReportFundsExpendituresCategoryLocalization
                {
                    EntityId = l.EntityId,
                    LanguageId = l.LanguageId,
                    Name = l.Name,
                    TranslationStatus = l.TranslationStatus,
                    CreatedAt = l.CreatedAt,
                })).ToArray();
            await _repositoryWrapper.BackupReportFundsExpendituresCategoryLocalizationsRepository
                .CreateRangeAsync(backupCategoryLocalizations);

            var backupFundsRecords = fundsRecords.Select(r => new BackupReportFundsExpendituresRecord
            {
                Id = r.Id,
                CategoryId = r.CategoryId,
                Type = r.Type,
                ReportingYear = r.ReportingYear,
                AmountUah = r.AmountUah,
                AmountUsd = r.AmountUsd,
                CreatedAt = r.CreatedAt,
            }).ToArray();
            await _repositoryWrapper.BackupReportFundsExpendituresRecordsRepository
                .CreateRangeAsync(backupFundsRecords);

            var backupProgramRecords = programRecords.Select(r => new BackupReportProgramExpendituresRecord
            {
                Id = r.Id,
                HippotherapyProgramCategoryId = r.HippotherapyProgramCategoryId,
                ReportingYear = r.ReportingYear,
                AmountUah = r.AmountUah,
                AmountUsd = r.AmountUsd,
                CreatedAt = r.CreatedAt,
            }).ToArray();
            await _repositoryWrapper.BackupReportProgramExpendituresRecordsRepository
                .CreateRangeAsync(backupProgramRecords);

            await _repositoryWrapper.SaveChangesAsync();
            transaction.Complete();

            return Result.Ok(true);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<bool>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<bool>("Failed to publish report funds expenditures data.");
        }
    }

    private static string? FindEnglishLocalizationName(
        ICollection<ReportFundsExpendituresCategoryLocalization> localizations)
    {
        return localizations
            .FirstOrDefault(l => l.Language.Code == EnglishLanguageCode)?.Name;
    }

    private static string? FindEnglishDisclaimerTitle(
        List<ReportFundsExpendituresSettingsLocalization> localizations)
    {
        return localizations
            .FirstOrDefault(l => l.Language.Code == EnglishLanguageCode)?.DisclaimerTitle;
    }
}
