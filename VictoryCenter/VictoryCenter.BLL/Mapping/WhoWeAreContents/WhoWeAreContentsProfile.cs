using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.BLL.Mapping.WhoWeAreContents;

public class WhoWeAreContentsProfile : Profile
{
    public WhoWeAreContentsProfile()
    {
        CreateMap<WhoWeAreContent, WhoWeAreContentDto>()
            .Include<ImageContent, ImageContentDto>()
            .Include<TitleContent, TitleContentDto>()
            .Include<DescriptionContent, DescriptionContentDto>()
            .Include<CardContent, CardContentDto>();

        CreateMap<ImageContent, ImageContentDto>();
        CreateMap<TitleContent, TitleContentDto>();
        CreateMap<DescriptionContent, DescriptionContentDto>();
        CreateMap<CardContent, CardContentDto>();
    }
}
