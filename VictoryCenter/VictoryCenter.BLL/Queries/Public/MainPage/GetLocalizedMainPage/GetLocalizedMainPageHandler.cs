using System.Globalization;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Public.MainPage.GetLocalizedMainPage;

public class GetLocalizedMainPageHandler : IRequestHandler<GetLocalizedMainPageQuery, Result<LocalizedMainPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetLocalizedMainPageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<LocalizedMainPageDto>> Handle(
        GetLocalizedMainPageQuery request,
        CancellationToken cancellationToken)
    {
        var mainPageEntity = await _repositoryWrapper.MainPageRepository
            .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
            {
                Include = q => q
                    .Include(e => e.Image)
                    .Include(e => e.Localizations)
                    .Include(e => e.MainAboutUs).ThenInclude(a => a!.Localizations)
                    .Include(e => e.MainPartners).ThenInclude(p => p!.Localizations)
                    .Include(e => e.MainDonations).ThenInclude(d => d!.Image)
                    .Include(e => e.MainDonations).ThenInclude(d => d!.Localizations)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Image)
                    .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Localizations)
                    .Include(e => e.ImpactStatistics)
                        .ThenInclude(s => s!.Metrics.Where(m => !m.IsHidden).OrderBy(m => m.Priority))
                        .ThenInclude(m => m.Localizations),
                AsNoTracking = true
            });

        if (mainPageEntity is null)
        {
            return Result.Fail<LocalizedMainPageDto>(ErrorMessagesConstants.NotFound());
        }

        return Result.Ok(MapLocalizedMainPage(mainPageEntity, request.LanguageId));
    }

    private LocalizedMainPageDto MapLocalizedMainPage(MainPageEntity mainPage, long? languageId)
    {
        var localization = FindLocalization(mainPage.Localizations, languageId);

        return new LocalizedMainPageDto
        {
            Id = mainPage.Id,
            LanguageId = languageId,
            Title = Resolve(localization?.Title, mainPage.Title),
            Description = Resolve(localization?.Description, mainPage.Description),
            Image = MapImage(mainPage.Image),
            MainAboutUs = MapLocalizedMainAboutUs(mainPage.MainAboutUs, languageId),
            MainPartners = MapLocalizedMainPartners(mainPage.MainPartners, languageId),
            MainDonations = MapLocalizedMainDonations(mainPage.MainDonations, languageId),
            ImpactStatistics = MapLocalizedImpactStatistics(mainPage.ImpactStatistics, languageId)
        };
    }

    private LocalizedMainAboutUsDto? MapLocalizedMainAboutUs(MainAboutUs? mainAboutUs, long? languageId)
    {
        if (mainAboutUs is null)
        {
            return null;
        }

        var localization = FindLocalization(mainAboutUs.Localizations, languageId);

        return new LocalizedMainAboutUsDto
        {
            Id = mainAboutUs.Id,
            Title = Resolve(localization?.Title, mainAboutUs.Title),
            Description = Resolve(localization?.Description, mainAboutUs.Description)
        };
    }

    private LocalizedMainPartnersDto? MapLocalizedMainPartners(MainPartners? mainPartners, long? languageId)
    {
        if (mainPartners is null)
        {
            return null;
        }

        var localization = FindLocalization(mainPartners.Localizations, languageId);

        return new LocalizedMainPartnersDto
        {
            Id = mainPartners.Id,
            Title = Resolve(localization?.Title, mainPartners.Title),
            Description = Resolve(localization?.Description, mainPartners.Description)
        };
    }

    private LocalizedMainDonationsDto? MapLocalizedMainDonations(MainDonations? mainDonations, long? languageId)
    {
        if (mainDonations is null)
        {
            return null;
        }

        var localization = FindLocalization(mainDonations.Localizations, languageId);

        return new LocalizedMainDonationsDto
        {
            Id = mainDonations.Id,
            Title = Resolve(localization?.Title, mainDonations.Title),
            Description = Resolve(localization?.Description, mainDonations.Description),
            Image = MapImage(mainDonations.Image)
        };
    }

    private LocalizedImpactStatisticDto? MapLocalizedImpactStatistics(
        ImpactStatistics? impactStatistics,
        long? languageId)
    {
        if (impactStatistics is null)
        {
            return null;
        }

        var localization = FindLocalization(impactStatistics.Localizations, languageId);

        return new LocalizedImpactStatisticDto
        {
            Id = impactStatistics.Id,
            Title = Resolve(localization?.Title, impactStatistics.Title),
            Image = MapImage(impactStatistics.Image),
            Metrics = impactStatistics.Metrics
                .OrderBy(m => m.Priority)
                .Select(metric => MapLocalizedMetric(metric, languageId))
                .ToList()
        };
    }

    private static LocalizedMetricDto MapLocalizedMetric(Metric metric, long? languageId)
    {
        var localization = FindLocalization(metric.Localizations, languageId);

        return new LocalizedMetricDto
        {
            Id = metric.Id,
            Value = Resolve(localization?.Value, metric.Value.ToString(CultureInfo.InvariantCulture)),
            Name = Resolve(localization?.Name, metric.Name),
            Type = metric.Type,
            Prefix = metric.Prefix,
            IsAutoSynced = metric.IsAutoSynced,
            IsHidden = metric.IsHidden,
            Priority = metric.Priority
        };
    }

    private ImageDto? MapImage(Image? image)
    {
        return image is null ? null : _mapper.Map<ImageDto>(image);
    }

    private static TLocalization? FindLocalization<TLocalization>(
        IEnumerable<TLocalization> localizations,
        long? languageId)
        where TLocalization : class
    {
        return languageId.HasValue
            ? localizations.FirstOrDefault(localization => GetLanguageId(localization) == languageId.Value)
            : null;
    }

    private static long GetLanguageId<TLocalization>(TLocalization localization)
        where TLocalization : class
    {
        return localization switch
        {
            MainPageLocalization mainPageLocalization => mainPageLocalization.LanguageId,
            MainAboutUsLocalization mainAboutUsLocalization => mainAboutUsLocalization.LanguageId,
            MainPartnersLocalization mainPartnersLocalization => mainPartnersLocalization.LanguageId,
            MainDonationsLocalization mainDonationsLocalization => mainDonationsLocalization.LanguageId,
            ImpactStatisticsLocalization impactStatisticsLocalization => impactStatisticsLocalization.LanguageId,
            MetricLocalization metricLocalization => metricLocalization.LanguageId,
            _ => throw new ArgumentOutOfRangeException(nameof(localization), localization, null)
        };
    }

    private static string Resolve(string? localizedValue, string sourceValue)
    {
        return string.IsNullOrWhiteSpace(localizedValue)
            ? sourceValue
            : localizedValue;
    }
}
