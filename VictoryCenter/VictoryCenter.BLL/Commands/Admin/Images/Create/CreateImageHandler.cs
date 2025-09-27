using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Images.Create;

public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<ImageDto>>
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

    public async Task<Result<ImageDto>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var fileName = Guid.NewGuid().ToString().Replace("-", "");

            using TransactionScope transaction = _repositoryWrapper.BeginTransaction();

            Image image = _mapper.Map<Image>(request.CreateImageDto);
            image.BlobName = fileName;
            image.CreatedAt = DateTimeOffset.UtcNow;

            Image createdImage = await _repositoryWrapper.ImageRepository.CreateAsync(image);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ImageDto>(ImageConstants.FailToSaveImageInDatabase);
            }

            await _blobService.SaveFileInStorageAsync(request.CreateImageDto.Base64, fileName, request.CreateImageDto.MimeType);

            ImageDto? response = _mapper.Map<ImageDto>(createdImage);

            transaction.Complete();

            return Result.Ok(response);
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
