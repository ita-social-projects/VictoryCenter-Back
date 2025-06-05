using AutoMapper;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Categories;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();
    }
}
