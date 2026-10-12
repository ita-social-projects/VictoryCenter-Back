using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageParticipantsSection;

public class HippotherapyLandingPageParticipantsSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageParticipantsSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageParticipantsSectionLocalizationDto, HippotherapyLandingPageParticipantsSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto, HippotherapyLandingPageParticipantsSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageParticipantsSectionLocalization, HippotherapyLandingPageParticipantsSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
