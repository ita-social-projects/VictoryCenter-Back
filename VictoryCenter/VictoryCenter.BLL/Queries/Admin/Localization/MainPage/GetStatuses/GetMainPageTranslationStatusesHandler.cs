using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetStatuses;

public class GetMainPageTranslationStatusesHandler
    : IRequestHandler<GetMainPageTranslationStatusesQuery, Result<List<MainPageTranslationStatusDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetMainPageTranslationStatusesHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<MainPageTranslationStatusDto>>> Handle(
        GetMainPageTranslationStatusesQuery request,
        CancellationToken cancellationToken)
    {
        var mainPage = await _repositoryWrapper.MainPageRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
            {
                Filter = e => e.Id == request.EntityId,
                Include = query => query
                    .Include(e => e.Localizations)
                    .Include(e => e.MainAboutUs).ThenInclude(a => a!.Localizations)
                    .Include(e => e.MainPartners).ThenInclude(p => p!.Localizations)
                    .Include(e => e.MainDonations).ThenInclude(d => d!.Localizations)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Localizations)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Metrics).ThenInclude(m => m.Localizations),
                AsNoTracking = true
            });

        if (mainPage is null)
        {
            return Result.Fail<List<MainPageTranslationStatusDto>>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(MainPageEntity)));
        }

        var statuses = new List<MainPageTranslationStatusDto>
        {
            BuildStatus(
                MainPageLocalizationBlock.Title,
                mainPage.Id,
                request.LanguageId,
                mainPage.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus),

            BuildStatus(
                MainPageLocalizationBlock.AboutUs,
                mainPage.MainAboutUs?.Id,
                request.LanguageId,
                mainPage.MainAboutUs?.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus),

            BuildStatus(
                MainPageLocalizationBlock.Partners,
                mainPage.MainPartners?.Id,
                request.LanguageId,
                mainPage.MainPartners?.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus),

            BuildStatus(
                MainPageLocalizationBlock.Donations,
                mainPage.MainDonations?.Id,
                request.LanguageId,
                mainPage.MainDonations?.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus),

            BuildStatus(
                MainPageLocalizationBlock.ImpactStatistics,
                mainPage.ImpactStatistics?.Id,
                request.LanguageId,
                mainPage.ImpactStatistics?.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus)
        };

        statuses.AddRange(GetMetricStatuses(mainPage.ImpactStatistics?.Metrics, request.LanguageId));

        return Result.Ok(statuses);
    }

    private static MainPageTranslationStatusDto BuildStatus<TEntityLocalization>(
        MainPageLocalizationBlock block,
        long? entityId,
        long languageId,
        IEnumerable<TEntityLocalization>? localizations,
        Func<TEntityLocalization, long> languageIdSelector,
        Func<TEntityLocalization, TranslationStatus> translationStatusSelector)
        where TEntityLocalization : class
    {
        var localization = localizations?.FirstOrDefault(l => languageIdSelector(l) == languageId);

        return new MainPageTranslationStatusDto
        {
            Block = block,
            EntityId = entityId,
            LanguageId = languageId,
            TranslationStatus = localization is null
                ? null
                : translationStatusSelector(localization)
        };
    }

    private static IEnumerable<MainPageTranslationStatusDto> GetMetricStatuses(
        IEnumerable<Metric>? metrics,
        long languageId)
    {
        if (metrics is null)
        {
            yield break;
        }

        foreach (var metric in metrics.OrderBy(m => m.Type))
        {
            yield return BuildStatus(
                ToLocalizationBlock(metric.Type),
                metric.Id,
                languageId,
                metric.Localizations,
                l => l.LanguageId,
                l => l.TranslationStatus);
        }
    }

    private static MainPageLocalizationBlock ToLocalizationBlock(MetricType metricType)
    {
        return metricType switch
        {
            MetricType.Partners => MainPageLocalizationBlock.MetricPartners,
            MetricType.Programs => MainPageLocalizationBlock.MetricPrograms,
            MetricType.Raised => MainPageLocalizationBlock.MetricRaised,
            MetricType.TherapyHours => MainPageLocalizationBlock.MetricTherapyHours,
            _ => throw new ArgumentOutOfRangeException(nameof(metricType), metricType, null)
        };
    }
}
