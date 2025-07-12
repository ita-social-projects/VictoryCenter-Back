using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Images.GetByName;

public class GetImageByNameHandler : IRequestHandler<GetImageByNameQuery, Result<ImageDTO>>
{
    private IBlobService _blobService;
    private IRepositoryWrapper _repositoryWrapper;
    private IMapper _mapper;

    public GetImageByNameHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ImageDTO>> Handle(GetImageByNameQuery request, CancellationToken cancellationToken)
    {
        var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>()
        {
            Filter = e => e.BlobName == request.name
        });
        if (image is null)
        {
            return Result.Fail<ImageDTO>("image not found");
        }

        if (image.BlobName == null)
        {
            return Result.Fail<ImageDTO>("image not found");
        }

        image.Base64 = _blobService.FindFileInStorageAsBase64(image.BlobName, image.MimeType);
        var result = _mapper.Map<ImageDTO>(image);
        return Result.Ok(result);
    }
}
