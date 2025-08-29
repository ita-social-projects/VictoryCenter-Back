using AutoMapper;
using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.DAL.Entities.AboutUsContents;

namespace VictoryCenter.BLL.Mapping.AboutUsContents;

public class AboutUsContentsProfile : Profile
{
    public AboutUsContentsProfile()
    {
        CreateMap<AboutUsContent, AboutUsContentDto>()
            .Include<ImageContent, ImageContentDto>()
            .Include<TitleContent, TitleContentDto>()
            .Include<DescriptionContent, DescriptionContentDto>();

        CreateMap<ImageContent, ImageContentDto>();
        CreateMap<TitleContent, TitleContentDto>();
        CreateMap<DescriptionContent, DescriptionContentDto>();
    }
}
