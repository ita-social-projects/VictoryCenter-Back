using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamMembers;

public class TeamMembersProfile : Profile
{
    public TeamMembersProfile()
    {
        CreateMap<CreateTeamMemberDto, TeamMember>();

        CreateMap<TeamMember, TeamMemberDto>();

        CreateMap<UpdateTeamMemberDto, TeamMember>();

        CreateMap<TeamMember, PublishedTeamMembersDto>();
    }
}
