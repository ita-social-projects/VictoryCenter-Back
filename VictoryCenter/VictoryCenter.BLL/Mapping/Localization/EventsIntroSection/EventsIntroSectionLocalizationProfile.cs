using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.EventsIntroSection;

public class EventsIntroSectionLocalizationProfile : Profile
{
    public EventsIntroSectionLocalizationProfile()
    {
        CreateMap<CreateEventsIntroSectionLocalizationDto, EventsIntroSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateEventsIntroSectionLocalizationDto, EventsIntroSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<EventsIntroSectionLocalization, EventsIntroSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
