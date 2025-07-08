using FluentResults;
using MediatR;
using VictoryCenter.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;

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
        var entityToDelete =
            await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query.Include(img => img.TeamMember!)
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>($"Image with ID {request.Id} not found.");
        }

        if (!string.IsNullOrEmpty(entityToDelete.BlobName))
        {
            _blobService.DeleteFileInStorage(entityToDelete.BlobName, entityToDelete.MimeType);
        }

        _repositoryWrapper.ImageRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>("Failed to delete image.");
    }
}
