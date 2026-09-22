using System.Globalization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.FundsMetricSync;

public interface IRaisedFundsMetricSyncService
{
    Task<bool> ApplySyncAsync(Metric raisedMetric, CancellationToken cancellationToken);
}

public class RaisedFundsMetricSyncService : IRaisedFundsMetricSyncService
{
    private const string EnglishLanguageCode = "en";

    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;

    public RaisedFundsMetricSyncService(IRepositoryWrapper repositoryWrapper, TimeProvider timeProvider)
    {
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
    }

    public async Task<bool> ApplySyncAsync(Metric raisedMetric, CancellationToken cancellationToken)
    {
        if (raisedMetric.Type != MetricType.Raised || !raisedMetric.IsAutoSynced)
        {
            return false;
        }

        var (incomeUahTotal, incomeUsdTotal, _, _, _, _) =
            await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.GetSummaryAsync();

        var changed = false;

        var newValue = ToMetricValue(incomeUahTotal);
        if (raisedMetric.Value != newValue)
        {
            raisedMetric.Value = newValue;
            changed = true;
        }

        var englishLanguage = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = language => language.Code == EnglishLanguageCode
            });

        if (englishLanguage is null)
        {
            return changed;
        }

        var newUsdValue = incomeUsdTotal.ToString(CultureInfo.InvariantCulture);

        var englishLocalization = raisedMetric.Localizations
            .FirstOrDefault(localization => localization.LanguageId == englishLanguage.Id);

        if (englishLocalization is null)
        {
            await _repositoryWrapper.MetricLocalizationsRepository.CreateAsync(
                 new MetricLocalization
                 {
                     EntityId = raisedMetric.Id,
                     LanguageId = englishLanguage.Id,
                     Value = newUsdValue,
                     TranslationStatus = TranslationStatus.Relevant,
                     CreatedAt = _timeProvider.GetUtcNow(),
                 });

            changed = true;
        }
        else if (englishLocalization.Value != newUsdValue)
        {
            englishLocalization.Value = newUsdValue;
            changed = true;
        }
        else
        {
        }

        return changed;
    }

    private static int ToMetricValue(decimal value)
    {
        var roundedValue = Math.Round(value, 0, MidpointRounding.AwayFromZero);

        if (roundedValue > int.MaxValue)
        {
            return int.MaxValue;
        }

        if (roundedValue < int.MinValue)
        {
            return int.MinValue;
        }

        return (int)roundedValue;
    }
}
