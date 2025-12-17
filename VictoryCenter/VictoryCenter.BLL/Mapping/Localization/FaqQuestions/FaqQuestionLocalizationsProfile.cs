using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.FaqQuestions;

public class FaqQuestionLocalizationsProfile : Profile
{
    public FaqQuestionLocalizationsProfile()
    {
        CreateMap<CreateFaqQuestionLocalizationDto, FaqQuestionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateFaqQuestionLocalizationDto, FaqQuestionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<FaqQuestionLocalization, FaqQuestionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
