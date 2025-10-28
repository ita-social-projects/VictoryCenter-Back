using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HippotherapyProgramCategories;

public class HippotherapyProgramCategoriesProfile : Profile
{
    public HippotherapyProgramCategoriesProfile()
    {
        CreateMap<HippotherapyProgramCategory, HippotherapyProgramCategoryDto>();
        CreateMap<CreateHippotherapyProgramCategoryDto, HippotherapyProgramCategory>();
        CreateMap<UpdateHippotherapyProgramCategoryDto, HippotherapyProgramCategory>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<HippotherapyProgramCategory, ProgramCategoryShortDto>();
    }
}
