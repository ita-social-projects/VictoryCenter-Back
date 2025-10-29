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
        CreateMap<TeamCategory, TeamCategoryDto>();
        CreateMap<UpdateTeamCategoryDto, TeamCategory>();

        CreateMap<TeamCategory, CategoryWithPublishedTeamMembersDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));
    }
}
