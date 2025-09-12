using AutoMapper;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class BlobToUrlResolver : IValueResolver<Image, ImageDto, string>
{
    private readonly IBlobService _blobService;
    public BlobToUrlResolver(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public string Resolve(Image source, ImageDto destination, string destMember, ResolutionContext context)
    {
        return _blobService.GetFileUrl(source.BlobName, source.MimeType);
    }
}
