using AutoMapper;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamMembers;

public class TeamMemberProfile : Profile
{
    public TeamMemberProfile()
    {
        CreateMap<TeamMember, TeamMemberDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
    }
}
