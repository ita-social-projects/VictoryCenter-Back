using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Images.Update;

public class UpdateImageHandler : BaseHandler<UpdateImageCommand, ImageDto>
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

    public override async Task<ImageDto> HandleRequest(UpdateImageCommand request, CancellationToken cancellationToken)
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
                throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
            }

            using TransactionScope transaction = _repositoryWrapper.BeginTransaction();

            var previousType = imageEntity.MimeType;
            imageEntity.MimeType = request.UpdateImageDto.MimeType!;

            _repositoryWrapper.ImageRepository.Update(imageEntity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Image)));
            }

            var updatedBlobName = await _blobService.UpdateFileInStorageAsync(
                imageEntity.BlobName,
                previousType,
                request.UpdateImageDto.Base64!,
                imageEntity.BlobName,
                request.UpdateImageDto.MimeType!);

            imageEntity.BlobName = updatedBlobName;

            ImageDto resultDto = _mapper.Map<Image, ImageDto>(imageEntity);

            transaction.Complete();

            return resultDto;
        }
        catch (BlobStorageException e)
        {
            throw new Exception(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
