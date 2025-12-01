using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetByName;

public class GetImageByNameHandler : IRequestHandler<GetImageByNameQuery, Result<ImageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetImageByNameHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ImageDto>> Handle(GetImageByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Image? image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = e => e.BlobName == request.Name
            });

            if (image is null)
            {
                return Result.Fail<ImageDto>(ImageConstants.ImageNotFoundGeneric);
            }

            if (string.IsNullOrEmpty(image.BlobName))
            {
                return Result.Fail<ImageDto>(ImageConstants.ImageDataNotAvailable);
            }

            ImageDto? result = _mapper.Map<ImageDto>(image);
            return Result.Ok(result);
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<ImageDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
