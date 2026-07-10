using AutoMapper;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Mapping.EventNews;

public class EventNewsProfile : Profile
{
    public EventNewsProfile()
    {
        CreateMap<EventNewsEntity, PublishedEventNewsDto>();

        CreateMap<EventNewsCategory, EventNewsCategoryDto>();

        CreateMap<EventNewsLocalization, PublishedEventNewsLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
