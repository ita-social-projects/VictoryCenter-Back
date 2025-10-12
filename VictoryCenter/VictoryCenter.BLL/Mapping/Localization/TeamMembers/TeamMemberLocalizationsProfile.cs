using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Localization.TeamMembers;

public class TeamMemberLocalizationsProfile : Profile
{
    public TeamMemberLocalizationsProfile()
    {
        CreateMap<CreateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.TeamMemberId));

        CreateMap<UpdateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.TeamMemberId));

        CreateMap<TeamMemberLocalization, TeamMemberLocalizationDto>()
            .ForMember(dest => dest.TeamMemberId, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.LocalizationLanguageDto, opt => opt.MapFrom(src => src.Language));
    }
}
