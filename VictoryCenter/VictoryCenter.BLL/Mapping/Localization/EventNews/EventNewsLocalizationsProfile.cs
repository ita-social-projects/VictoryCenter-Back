using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.EventNews;

public class EventNewsLocalizationsProfile : Profile
{
    public EventNewsLocalizationsProfile()
    {
        CreateMap<CreateEventNewsLocalizationDto, EventNewsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateEventNewsLocalizationDto, EventNewsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<EventNewsLocalization, EventNewsLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
