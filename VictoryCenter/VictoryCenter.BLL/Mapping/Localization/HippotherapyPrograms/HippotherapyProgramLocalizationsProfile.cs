using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyPrograms;

public class HippotherapyProgramLocalizationsProfile : Profile
{
    public HippotherapyProgramLocalizationsProfile()
    {
        CreateMap<CreateHippotherapyProgramLocalizationDto, HippotherapyProgramLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyProgramLocalizationDto, HippotherapyProgramLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyProgramLocalization, HippotherapyProgramLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
