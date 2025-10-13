using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HypotherapyProgramCategories;

public class HypotherapyProgramCategoriesProfile : Profile
{
    public HypotherapyProgramCategoriesProfile()
    {
        CreateMap<HippotherapyProgramCategory, HypotherapyProgramCategoryDto>();
        CreateMap<CreateHypotherapyProgramCategoryDto, HippotherapyProgramCategory>();
        CreateMap<UpdateHypotherapyProgramCategoryDto, HippotherapyProgramCategory>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<HippotherapyProgramCategory, ProgramCategoryShortDto>();
    }
}
