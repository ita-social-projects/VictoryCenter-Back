using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Images.GetById;

public class GetImageByIdHandler : IRequestHandler<GetImageByIdQuery, Result<ImageDTO>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;

    public GetImageByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
    }

    public async Task<Result<ImageDTO>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>()
            {
                Filter = e => e.Id == request.Id
            });

            if (image is null)
            {
                return Result.Fail<ImageDTO>(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
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
            return Result.Fail<ImageDTO>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
