using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.WhoWeAreContents;

public class WhoWeAreContentLocalizationsProfile : Profile
{
    public WhoWeAreContentLocalizationsProfile()
    {
        CreateMap<CreateWhoWeAreContentLocalizationDto, WhoWeAreContentLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateWhoWeAreContentLocalizationDto, WhoWeAreContentLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<WhoWeAreContentLocalization, WhoWeAreContentLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
