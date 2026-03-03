using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyProgramSections;

public class ProgramSectionContentLocalizationsProfile : Profile
{
    public ProgramSectionContentLocalizationsProfile()
    {
        CreateMap<CreateHippotherapyProgramSectionContentLocalizationDto, ProgramSectionContentLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyProgramSectionContentLocalizationDto, ProgramSectionContentLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<ProgramSectionContentLocalization, HippotherapyProgramSectionContentLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
