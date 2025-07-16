using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.TeamMembers;

public class TeamMemberImageResolver : IValueResolver<TeamMember, TeamMemberDto, ImageDTO?>
{
    private readonly IBlobService _blobService;

    public TeamMemberImageResolver(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public ImageDTO? Resolve(TeamMember source, TeamMemberDto destination, ImageDTO? destMember, ResolutionContext context)
    {
        if (source.Image == null)
        {
            return null;
        }

        return new ImageDTO
        {
            Id = source.Image.Id,
            BlobName = source.Image.BlobName,
            MimeType = source.Image.MimeType,
            Base64 = _blobService.FindFileInStorageAsBase64(source.Image.BlobName, source.Image.MimeType)
        };
    }
}
