using FluentResults;
using MediatR;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions;

namespace VictoryCenter.BLL.Commands.Images.Delete;

public class DeleteImageHandler : IRequestHandler<DeleteImageCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;

    public DeleteImageHandler(IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public async Task<Result<long>> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = entity => entity.Id == request.Id,
            });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.ImageRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(ImageConstants.FailToDeleteImage);
            }

            if (!string.IsNullOrEmpty(entityToDelete.BlobName))
            {
                 _blobService.DeleteFileInStorage(entityToDelete.BlobName, entityToDelete.MimeType);
            }

            transaction.Complete();

            return Result.Ok(entityToDelete.Id);
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<long>(ErrorMessagesConstants.BlobStorageError(e.Message) );
        }
    }
}
