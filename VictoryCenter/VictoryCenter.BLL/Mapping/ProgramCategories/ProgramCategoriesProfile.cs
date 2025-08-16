using AutoMapper;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ProgramCategories;

public class ProgramCategoriesProfile : Profile
{
    public ProgramCategoriesProfile()
    {
        CreateMap<ProgramCategory, ProgramCategoryDto>();
        CreateMap<CreateProgramCategoryDto, ProgramCategory>();
        CreateMap<UpdateProgramCategoryDto, ProgramCategory>();
        CreateMap<ProgramCategory, ProgramCategoryShortDto>();
    }
}
