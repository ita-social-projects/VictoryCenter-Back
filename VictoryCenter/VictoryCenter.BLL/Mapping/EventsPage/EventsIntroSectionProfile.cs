using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.DTOs.Public.EventsPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.EventsPage;

public class EventsIntroSectionProfile : Profile
{
    public EventsIntroSectionProfile()
    {
        CreateMap<EventsIntroSection, EventsIntroSectionDto>();
        CreateMap<EventsIntroSectionLocalization, PublicEventsIntroSectionLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
