using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Images.GetById;

public class GetImageByIdHandler : IRequestHandler<GetImageByIdQuery, Result<ImageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetImageByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ImageDto>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Image? image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = e => e.Id == request.Id
            });

            if (image is null)
            {
                return Result.Fail<ImageDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
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
