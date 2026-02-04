using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.TeamCategories;
public class TeamCategoryLocalizationProfile : Profile
{
    public TeamCategoryLocalizationProfile()
    {
        CreateMap<CreateTeamCategoryLocalizationDto, TeamCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateTeamCategoryLocalizationDto, TeamCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<TeamCategoryLocalization, TeamCategoryLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
