using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamCategories;

public class TeamCategoriesProfile : Profile
{
    public TeamCategoriesProfile()
    {
        CreateMap<CreateTeamCategoryDto, TeamCategory>();
        CreateMap<UpdateTeamCategoryDto, TeamCategory>();

        CreateMap<TeamCategory, TeamCategoryDto>()
            .ForMember(dest => dest.TeamMembersCount, opt => opt.MapFrom(src => src.TeamMembers.Count));

        CreateMap<TeamCategory, CategoryWithPublishedTeamMembersDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));
    }
}
