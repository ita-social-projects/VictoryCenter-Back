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
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.Cards, opt => opt.Ignore());

        CreateMap<UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto, HippotherapyLandingPageParticipantCardLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<HippotherapyLandingPageParticipantCardLocalization, HippotherapyLandingPageParticipantCardLocalizationItemDto>()
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.EntityId));
    }
}
