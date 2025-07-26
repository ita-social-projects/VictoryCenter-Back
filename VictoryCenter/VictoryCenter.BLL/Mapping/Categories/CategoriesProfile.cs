using AutoMapper;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Categories;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<Category, CategoryWithPublishedTeamMembersDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));
    }
}
