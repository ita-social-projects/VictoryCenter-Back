using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Mapping.Images.ImageResolver;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class ImagesProfile : Profile
{
    public ImagesProfile()
    {
        CreateMap<CreateImageDTO, Image>();
        CreateMap<UpdateImageDTO, Image>();
        CreateMap<Image, ImageDTO>().ForMember(d => d.Url, o => o.MapFrom<BlobToUrlResolver>());
    }
}
