using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.EventsPage;

public class EventsIntroSectionProfile : Profile
{
    public EventsIntroSectionProfile()
    {
        CreateMap<EventsIntroSection, EventsIntroSectionDto>();
    }
}
