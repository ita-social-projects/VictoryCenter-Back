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
        CreateMap<CreateHippotherapyProgramCategoryDto, HippotherapyProgramCategory>()
            .ForMember(dest => dest.Programs, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateHippotherapyProgramCategoryDto, HippotherapyProgramCategory>()
            .ForMember(dest => dest.Programs, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<HippotherapyProgramCategory, ProgramCategoryShortDto>();
    }
}
