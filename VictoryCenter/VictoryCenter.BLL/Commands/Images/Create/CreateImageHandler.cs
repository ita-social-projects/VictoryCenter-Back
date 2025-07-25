using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Images.Create;

public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<ImageDTO>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateImageCommand> _validator;

    public CreateImageHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateImageCommand> validator)
    {
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<ImageDTO>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var fileName = Guid.NewGuid().ToString().Replace("-", "");

            using var transaction = _repositoryWrapper.BeginTransaction();

            // Спочатку зберігаємо в базу даних
            Image image = _mapper.Map<Image>(request.CreateImageDto);
            image.BlobName = fileName;
            image.CreatedAt = DateTime.UtcNow;

            Image createdImage = await _repositoryWrapper.ImageRepository.CreateAsync(image);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ImageDTO>(ImageConstants.FailToSaveImageInDatabase);
            }

            // Потім зберігаємо файл в blob storage
            await _blobService.SaveFileInStorageAsync(request.CreateImageDto.Base64, fileName, request.CreateImageDto.MimeType);

            // Повертаємо результат з завантаженим Base64
            var response = _mapper.Map<ImageDTO>(createdImage);
            response.Base64 = await _blobService.FindFileInStorageAsBase64Async(response.BlobName, response.MimeType);

            // Комітимо транзакцію прямо перед поверненням
            transaction.Complete();

            return Result.Ok(response);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<ImageDTO>($"BlobStorage error: {e.Message}" );
        }
        catch (Exception)
        {
            return Result.Fail<ImageDTO>(ImageConstants.FailToSaveImageInStorage);
        }
    }
}
