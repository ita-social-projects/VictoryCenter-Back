using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Images.GetById;

public class GetImageByIdHandler : IRequestHandler<GetImageByIdQuery, Result<ImageDTO>>
{
    private IBlobService _blobService;
    private IRepositoryWrapper _repositoryWrapper;
    private IMapper _mapper;

    public GetImageByIdHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ImageDTO>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>()
        {
            Filter = e => e.Id == request.id
        });
        if (image is null)
        {
            return Result.Fail<ImageDTO>("image not found");
        }

        if (image.BlobName == null)
        {
            return Result.Fail<ImageDTO>("image not found");
        }

        var extension = image.MimeType.Split("/").Last();
        var fullPath = image.BlobName + "." + extension;
        image.Base64 = _blobService.FindFileInStorageAsBase64(fullPath);
        var result = _mapper.Map<ImageDTO>(image);
        return Result.Ok(result);
    }
}
