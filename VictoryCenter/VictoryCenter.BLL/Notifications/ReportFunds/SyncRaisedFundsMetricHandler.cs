using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Notifications.ReportFunds;

public class SyncRaisedFundsMetricHandler : INotificationHandler<ReportFundsChangedNotification>
{
    private const string EnglishLanguageCode = "en";

    private readonly IRepositoryWrapper _repositoryWrapper;

    public SyncRaisedFundsMetricHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task Handle(ReportFundsChangedNotification notification, CancellationToken cancellationToken)
    {
        var (incomeUahTotal, incomeUsdTotal, _, _, _, _) =
            await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.GetSummaryAsync();

        var raisedMetric = await _repositoryWrapper.MetricRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Metric>
            {
                AsNoTracking = false,
                Filter = metric => metric.Type == MetricType.Raised,
                Include = query => query
                    .Include(metric => metric.Localizations)
                    .ThenInclude(localization => localization.Language)
            });

        if (raisedMetric?.IsAutoSynced != true)
        {
            return;
        }

        raisedMetric.Value = ToMetricValue(incomeUahTotal);

        var englishLanguage = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = language => language.Code == EnglishLanguageCode
            });

        if (englishLanguage is null)
        {
            return;
        }

        var englishLocalization = raisedMetric.Localizations
            .FirstOrDefault(localization => localization.Language.Code == EnglishLanguageCode);

        if (englishLocalization is null)
        {
            await _repositoryWrapper.MetricLocalizationsRepository.CreateAsync(
                new MetricLocalization
                {
                    EntityId = raisedMetric.Id,
                    LanguageId = englishLanguage.Id,
                    Value = incomeUsdTotal.ToString(CultureInfo.InvariantCulture),
                    CreatedAt = DateTimeOffset.UtcNow
                });
        }
        else
        {
            englishLocalization.Value = incomeUsdTotal.ToString(CultureInfo.InvariantCulture);
            englishLocalization.TranslationStatus = TranslationStatus.Relevant;
        }

        await _repositoryWrapper.SaveChangesAsync();
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
