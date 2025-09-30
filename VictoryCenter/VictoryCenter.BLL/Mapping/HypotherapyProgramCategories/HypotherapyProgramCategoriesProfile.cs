using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HypotherapyProgramCategories;

public class HypotherapyProgramCategoriesProfile : Profile
{
    public HypotherapyProgramCategoriesProfile()
    {
        CreateMap<HypotherapyProgramCategory, HypotherapyProgramCategoryDto>();
        CreateMap<CreateHypotherapyProgramCategoryDto, HypotherapyProgramCategory>();
        CreateMap<UpdateHypotherapyProgramCategoryDto, HypotherapyProgramCategory>();
        CreateMap<HypotherapyProgramCategory, ProgramCategoryShortDto>();
    }
}
