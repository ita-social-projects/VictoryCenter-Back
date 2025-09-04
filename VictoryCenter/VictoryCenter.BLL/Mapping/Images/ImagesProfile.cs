using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class ImagesProfile : Profile
{
    public ImagesProfile()
    {
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();
        CreateMap<Image, ImageDto>().ForMember(d => d.Url, o => o.MapFrom<BlobToUrlResolver>());
    }
}
