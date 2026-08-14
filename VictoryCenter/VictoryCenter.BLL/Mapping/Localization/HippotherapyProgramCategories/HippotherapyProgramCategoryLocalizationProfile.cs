using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyProgramCategories;

public class HippotherapyProgramCategoryLocalizationProfile : Profile
{
    public HippotherapyProgramCategoryLocalizationProfile()
    {
        CreateMap<CreateHippotherapyProgramCategoryLocalizationDto, HippotherapyProgramCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyProgramCategoryLocalizationDto, HippotherapyProgramCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyProgramCategoryLocalization, HippotherapyProgramCategoryLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
