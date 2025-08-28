using AutoMapper;
using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.DAL.Entities.AboutUsContents;

namespace VictoryCenter.BLL.Mapping.AboutUsContents;

public class AboutUsContentsProfile : Profile
{
    public AboutUsContentsProfile()
    {
        CreateMap<AboutUsContent, AboutUsContentDto>()
            .IncludeAllDerived();
    }
}
