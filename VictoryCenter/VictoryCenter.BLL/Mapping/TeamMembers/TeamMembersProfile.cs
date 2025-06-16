using AutoMapper;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamMembers;

public class TeamMembersProfile : Profile
{
    public TeamMembersProfile()
    {
        CreateMap<TeamMember, TeamMemberDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null));
    }
}
