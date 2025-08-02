using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class BlobTobase64Resolver : IValueResolver<Image, ImageDTO, string>
{
    private readonly IBlobService _blobService;
    public BlobTobase64Resolver(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public string Resolve(Image source, ImageDTO destination, string destMember, ResolutionContext context)
    {
        try
        {
            return _blobService.GetFileUrl(source.BlobName, source.MimeType);
        }
        catch (Exception e)
        {
            throw new FileNotFoundException();
        }
    }
}
