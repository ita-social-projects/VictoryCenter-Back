using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Images.GetByName;

public class GetImageByNameHandler : IRequestHandler<GetImageByNameQuery, Result<ImageDTO>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

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
            Image? image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
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

            ImageDTO? result = _mapper.Map<ImageDTO>(image);
            return Result.Ok(result);
        }
        catch (BlobStorageException e)
        {
            var test = ErrorMessagesConstants.BlobStorageError(e.Message);
            return Result.Fail<ImageDTO>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
