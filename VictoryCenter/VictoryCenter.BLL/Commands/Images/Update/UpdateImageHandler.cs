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
            Image? imageEntity =
                await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = entity => entity.Id == request.id
                });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDTO>("Not found");
            }

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            if (!string.IsNullOrEmpty(request.updateImageDto.Base64) &&
                !string.IsNullOrEmpty(request.updateImageDto.MimeType))
            {
                var updatedBlobName = _blobService.UpdateFileInStorage(
                    imageEntity.BlobName,
                    imageEntity.MimeType,
                    request.updateImageDto.Base64,
                    imageEntity.BlobName,
                    request.updateImageDto.MimeType);

                imageEntity.BlobName = updatedBlobName;
                imageEntity.MimeType = request.updateImageDto.MimeType;
            }

            Image? image = _mapper.Map(request.updateImageDto, imageEntity);
            image.CreatedAt = imageEntity.CreatedAt;

            _repositoryWrapper.ImageRepository.Update(image);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                ImageDTO? resultDto = _mapper.Map<Image, ImageDTO>(image);
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
