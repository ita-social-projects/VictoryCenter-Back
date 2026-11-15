using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Errors;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Images.Delete;

public class DeleteImageHandler : IRequestHandler<DeleteImageCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteImageHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
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
                return Result.Fail<long>(new NotFoundError(ErrorMessagesConstants.NotFound(request.Id, typeof(Image))));
            }

            _repositoryWrapper.ImageRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Image)));
            }

            return Result.Ok(entityToDelete.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(Image)));
        }
    }
}
