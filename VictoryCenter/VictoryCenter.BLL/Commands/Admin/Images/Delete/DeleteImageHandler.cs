using System.Transactions;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Images.Delete;

public class DeleteImageHandler : BaseHandler<DeleteImageCommand, long>
{
    private readonly IBlobService _blobService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteImageHandler(IRepositoryWrapper repositoryWrapper, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public override async Task<long> HandleRequest(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Image? entityToDelete = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Image>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (entityToDelete is null)
            {
               throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(Image)));
            }

            using TransactionScope transaction = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.ImageRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Image)));
            }

            if (!string.IsNullOrEmpty(entityToDelete.BlobName))
            {
                _blobService.DeleteFileInStorage(entityToDelete.BlobName, entityToDelete.MimeType);
            }

            transaction.Complete();

            return entityToDelete.Id;
        }
        catch (BlobStorageException e)
        {
            throw new Exception(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
    }
}
