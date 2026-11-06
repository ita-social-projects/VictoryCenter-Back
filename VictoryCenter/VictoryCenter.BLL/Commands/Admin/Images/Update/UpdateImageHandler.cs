using System.Globalization;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Errors;

namespace VictoryCenter.BLL.Commands.Admin.Images.Update;

public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<ImageDto>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateImageHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private readonly IValidator<UpdateImageCommand> _validator;

    public UpdateImageHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateImageCommand> validator,
        IBlobService blobService,
        TimeProvider timeProvider,
        ILogger<UpdateImageHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _blobService = blobService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<Result<ImageDto>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Image? imageEntity = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDto>(NotFoundErrors.NotFound(request.Id, typeof(Image)));
            }

            var replacementBlobName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            await _blobService.SaveFileInStorageAsync(
                request.UpdateImageDto.Base64!,
                replacementBlobName,
                request.UpdateImageDto.MimeType!);

            imageEntity.BlobName = replacementBlobName;
            imageEntity.MimeType = request.UpdateImageDto.MimeType!;
            imageEntity.UpdatedAt = _timeProvider.GetUtcNow();

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ImageDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Image)));
            }

            ImageDto resultDto = _mapper.Map<Image, ImageDto>(imageEntity);

            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDto>(vex.Errors.Select(e => (IError)new ValidationError(e.PropertyName, e.ErrorMessage)));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<ImageDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
        catch (DbUpdateException exception)
        {
            _logger.LogWarning(exception, "Image metadata update failed for {ImageId}", request.Id);
            return Result.Fail<ImageDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Image)));
        }
    }
}
