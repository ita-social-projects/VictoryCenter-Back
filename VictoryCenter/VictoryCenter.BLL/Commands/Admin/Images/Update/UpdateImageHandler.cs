using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Images.Update;

public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<ImageDto>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateImageCommand> _validator;

    public UpdateImageHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateImageCommand> validator,
        IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<ImageDto>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Image? imageEntity = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = entity => entity.Id == request.Id
            });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            imageEntity.MimeType = request.UpdateImageDto.MimeType!;
            _repositoryWrapper.ImageRepository.Update(imageEntity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ImageDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Image)));
            }

            var updatedBlobName = await _blobService.UpdateFileInStorageAsync(
                imageEntity.BlobName,
                imageEntity.MimeType,
                request.UpdateImageDto.Base64!,
                imageEntity.BlobName,
                request.UpdateImageDto.MimeType!);

            imageEntity.BlobName = updatedBlobName;

            ImageDto resultDto = _mapper.Map<Image, ImageDto>(imageEntity);
            resultDto.Base64 = await _blobService.FindFileInStorageAsBase64Async(resultDto.BlobName, resultDto.MimeType);

            transaction.Complete();

            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<ImageDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
