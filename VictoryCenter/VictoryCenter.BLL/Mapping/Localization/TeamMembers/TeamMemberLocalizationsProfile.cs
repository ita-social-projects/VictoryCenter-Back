using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.TeamMembers;

public class TeamMemberLocalizationsProfile : Profile
{
    public TeamMemberLocalizationsProfile()
    {
        CreateMap<CreateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<TeamMemberLocalization, TeamMemberLocalizationDto>()
            .ForMember(dest => dest.LocalizatioInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
