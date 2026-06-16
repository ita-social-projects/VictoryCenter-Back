using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Public.MainPage;

public record LocalizedMainPageDto
{
    public long Id { get; init; }
    public long? LanguageId { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public LocalizedMainAboutUsDto? MainAboutUs { get; init; }
    public LocalizedMainPartnersDto? MainPartners { get; init; }
    public LocalizedMainDonationsDto? MainDonations { get; init; }
    public LocalizedImpactStatisticDto? ImpactStatistics { get; init; }
}

public record LocalizedMainAboutUsDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public record LocalizedMainPartnersDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public record LocalizedMainDonationsDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }
}

public record LocalizedImpactStatisticDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public ICollection<LocalizedMetricDto> Metrics { get; init; } = [];
}

public record LocalizedMetricDto
{
    public long Id { get; init; }
    public string Value { get; init; } = null!;
    public string Name { get; init; } = null!;
    public MetricType Type { get; init; }
    public MetricPrefix? Prefix { get; init; }
    public bool IsAutoSynced { get; init; }
    public bool IsHidden { get; init; }
    public long Priority { get; init; }
}
