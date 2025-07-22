using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;
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

            try
            {
                // Спочатку зберігаємо в базу даних
                Image image = _mapper.Map<Image>(request.CreateImageDto);
                image.BlobName = fileName;
                image.CreatedAt = DateTime.UtcNow;

                Image createdImage = await _repositoryWrapper.ImageRepository.CreateAsync(image);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<ImageDTO>(ImageConstants.FailToSaveImageInDatabase);
                }

                // Пробуєм тут зберегти файл в blob storage
                try
                {
                    await _blobService.SaveFileInStorageAsync(request.CreateImageDto.Base64, fileName, request.CreateImageDto.MimeType);
                }
                catch (Exception e)
                {
                    // Транзакція автоматично зробить rollback при dispose
                    return Result.Fail<ImageDTO>(ImageConstants.FailToSaveImageInStorage);
                }

                // Якщо все пройшло успішно, комітимо транзакцію
                transaction.Complete();

                // Повертаємо результат з завантаженим Base64
                var response = _mapper.Map<ImageDTO>(createdImage);
                response.Base64 = await _blobService.FindFileInStorageAsBase64Async(response.BlobName, response.MimeType);

                return Result.Ok(response);
            }
            catch (Exception e)
            {
                // Транзакція автоматично зробить rollback при dispose
                return Result.Fail<ImageDTO>(ImageConstants.FailToSaveImageInStorage);
            }
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
