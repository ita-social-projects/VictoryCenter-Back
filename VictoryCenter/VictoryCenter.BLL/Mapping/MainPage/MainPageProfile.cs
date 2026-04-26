using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Mapping.MainPage;

public class MainPageProfile : Profile
{
    public MainPageProfile()
    {
        CreateMap<CreateMainPageDto, MainPageEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        CreateMap<CreateMainAboutUsDto, MainAboutUs>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MainPageId, opt => opt.Ignore())
            .ForMember(dest => dest.MainPage, opt => opt.Ignore());

        CreateMap<CreateMainPartnersDto, MainPartners>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MainPageId, opt => opt.Ignore())
            .ForMember(dest => dest.MainPage, opt => opt.Ignore());

        CreateMap<CreateImpactStatisticDto, ImpactStatistics>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.MainPageId, opt => opt.Ignore())
            .ForMember(dest => dest.MainPage, opt => opt.Ignore())
            .ForMember(dest => dest.Localizations, opt => opt.Ignore());

        CreateMap<CreateMetricDto, Metric>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.StatisticId, opt => opt.Ignore())
            .ForMember(dest => dest.Statistics, opt => opt.Ignore())
            .ForMember(dest => dest.Localizations, opt => opt.Ignore());

        CreateMap<MainPageEntity, MainPageDto>();
        CreateMap<MainAboutUs, MainAboutUsDto>();
        CreateMap<MainPartners, MainPartnersDto>();
        CreateMap<ImpactStatistics, ImpactStatisticDto>();
        CreateMap<Metric, MetricDto>();

        CreateMap<UpdateMainPageDto, MainPageEntity>();
        CreateMap<UpdateMainAboutUsDto, MainAboutUs>();
        CreateMap<UpdateMainPartnersDto, MainPartners>();
        CreateMap<UpdateImpactStatisticDto, ImpactStatistics>();
        CreateMap<UpdateMetricDto, Metric>();

        CreateMap<ImpactStatisticsLocalization, ImpactStatisticLocalizationDto>();
        CreateMap<MetricLocalization, MetricLocalizationDto>();
    }
}
