using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Commands.Images.Update;

public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<ImageDTO>>
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

    public async Task<Result<ImageDTO>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Image? imageEntity = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = entity => entity.Id == request.id
            });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDTO>(ImageConstants.ImageNotFound(request.id));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            // Спочатку оновлюємо дані в БД
            imageEntity.MimeType = request.updateImageDto.MimeType!;

            var result = _repositoryWrapper.ImageRepository.Update(imageEntity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ImageDTO>(ImageConstants.FailToUpdateImage);
            }

            // Потім оновлюємо файл в blob storage
            var updatedBlobName = await _blobService.UpdateFileInStorageAsync(
                imageEntity.BlobName,
                imageEntity.MimeType,
                request.updateImageDto.Base64!,
                imageEntity.BlobName,
                request.updateImageDto.MimeType!);

            imageEntity.BlobName = updatedBlobName;

            // Повертаємо результат з завантаженим Base64
            ImageDTO resultDto = _mapper.Map<Image, ImageDTO>(imageEntity);
            resultDto.Base64 = await _blobService.FindFileInStorageAsBase64Async(resultDto.BlobName, resultDto.MimeType);

            // Комітимо транзакцію перед поверненням
            transaction.Complete();

            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception e)
        {
            return Result.Fail<ImageDTO>(ImageConstants.FailToUpdateImage);
        }
    }
}
