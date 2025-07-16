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
    private IRepositoryWrapper _repositoryWrapper;
    private IMapper _mapper;
    private IBlobService _blobService;

    public GetImageByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
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

        var result = _mapper.Map<ImageDTO>(image);
        result.Base64 = await _blobService.FindFileInStorageAsBase64Async(result.BlobName, result.MimeType);
        return Result.Ok(result);
    }
}
