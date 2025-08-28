using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Images.GetByName;

public class GetImageByNameHandler : IRequestHandler<GetImageByNameQuery, Result<ImageDTO>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;

    public GetImageByNameHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
    }

    public async Task<Result<ImageDTO>> Handle(GetImageByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = e => e.BlobName == request.Name
            });

            if (image is null)
            {
                return Result.Fail<ImageDTO>(ImageConstants.ImageNotFoundGeneric);
            }

            if (string.IsNullOrEmpty(image.BlobName))
            {
                return Result.Fail<ImageDTO>(ImageConstants.ImageDataNotAvailable);
            }

            image.Base64 = await _blobService.FindFileInStorageAsBase64Async(image.BlobName, image.MimeType);
            var result = _mapper.Map<ImageDTO>(image);
            return Result.Ok(result);
        }
        catch (BlobStorageException e)
        {
            var test = ErrorMessagesConstants.BlobStorageError(e.Message);
            return Result.Fail<ImageDTO>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
