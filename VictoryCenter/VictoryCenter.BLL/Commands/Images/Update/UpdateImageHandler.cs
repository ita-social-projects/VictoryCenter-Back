using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Images.Update;

public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<ImageDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateImageCommand> _validator;
    private readonly IBlobService _blobService;

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
            var imageEntity =
                await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = entity => entity.Id == request.updateImageDto.Id
                });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDTO>("Not found");
            }

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            // Оновлення файлу у blob storage, якщо передано новий base64
            if (!string.IsNullOrEmpty(request.updateImageDto.Base64) && !string.IsNullOrEmpty(request.updateImageDto.MimeType))
            {
                var newBlobName = imageEntity.BlobName.Split('.')[0]; // залишаємо стару назву без розширення
                var updatedBlobName = _blobService.UpdateFileInStorage(
                    imageEntity.BlobName,
                    request.updateImageDto.Base64,
                    newBlobName,
                    request.updateImageDto.MimeType);
                imageEntity.BlobName = updatedBlobName;
            }

            // Оновлення інших полів (наприклад, MimeType)
            _mapper.Map(request.updateImageDto, imageEntity);

            _repositoryWrapper.ImageRepository.Update(imageEntity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<Image, ImageDTO>(imageEntity);
                return Result.Ok(resultDto);
            }

            return Result.Fail<ImageDTO>("Failed to update image");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
