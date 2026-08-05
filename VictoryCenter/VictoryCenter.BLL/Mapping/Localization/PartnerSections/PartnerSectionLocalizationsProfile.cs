using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.PartnerSections;

public class PartnerSectionLocalizationsProfile : Profile
{
    public PartnerSectionLocalizationsProfile()
    {
        CreateMap<CreatePartnerSectionLocalizationDto, PartnerSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.LanguageId));

        CreateMap<UpdatePartnerSectionLocalizationDto, PartnerSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<PartnerSectionLocalization, PartnerSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.Partners, opt => opt.Ignore());

        CreateMap<PartnerSectionLocalization, PartnerSectionLocalizationSummaryDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));

        CreateMap<UpdatePartnerLocalizationItemDto, PartnerLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.PartnerId))
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<PartnerLocalization, PartnerLocalizationItemDto>()
            .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.EntityId));

        CreateMap<PartnerLocalization, PartnerLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
