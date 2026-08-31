using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.EventNewsCategories;

public class EventNewsCategoryProfile : Profile
{
    public EventNewsCategoryProfile()
    {
        CreateMap<EventNewsCategory, AdminEventNewsCategoryDto>()
            .ForMember(destination => destination.RelatedEventNewsCount, options => options.MapFrom(source => source.EventsNews.Count));
        CreateMap<EventNewsCategoryLocalization, AdminEventNewsCategoryLocalizationDto>()
            .ForMember(destination => destination.Language, options => options.MapFrom(source => source.Language));
    }
}
