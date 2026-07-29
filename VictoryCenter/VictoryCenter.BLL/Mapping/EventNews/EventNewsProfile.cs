using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using AdminEventNewsCategoryDto = VictoryCenter.BLL.DTOs.Admin.EventNews.EventNewsCategoryShortDto;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;
using PublicEventNewsCategoryDto = VictoryCenter.BLL.DTOs.Public.EventNews.EventNewsCategoryDto;

namespace VictoryCenter.BLL.Mapping.EventNews;

public class EventNewsProfile : Profile
{
    public EventNewsProfile()
    {
        CreateMap<EventNewsEntity, PublishedEventNewsDto>();
        CreateMap<EventNewsEntity, EventNewsDto>();

        CreateMap<CreateEventNewsDto, EventNewsEntity>()
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.PreviewImage, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundImage, opt => opt.Ignore())
            .ForMember(dest => dest.Localizations, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<EventNewsCategory, PublicEventNewsCategoryDto>();
        CreateMap<EventNewsCategory, AdminEventNewsCategoryDto>();

        CreateMap<EventNewsLocalization, PublishedEventNewsLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

        CreateMap<EventNewsLocalization, EventNewsLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
