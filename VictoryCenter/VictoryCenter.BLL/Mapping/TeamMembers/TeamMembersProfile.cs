using AutoMapper;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamMembers;

public class TeamMembersProfile : Profile
{
    public TeamMembersProfile()
    {
        CreateMap<CreateTeamMemberDto, TeamMember>()
            .ReverseMap();

        CreateMap<TeamMember, TeamMemberDto>()
            .ReverseMap();
    }
    
}
