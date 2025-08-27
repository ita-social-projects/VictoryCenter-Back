using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class ImagesProfile : Profile
{
    public ImagesProfile()
    {
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();
        CreateMap<Image, ImageDto>();
    }
}
