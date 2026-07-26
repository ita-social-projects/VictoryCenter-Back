using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Delete;

public class DeleteEventNewsCategoryHandler
    : IRequestHandler<DeleteEventNewsCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteEventNewsCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(
        DeleteEventNewsCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _repositoryWrapper.EventNewsCategoryRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategory>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

        if (category is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsCategory)));
        }

        var isInUse = await _repositoryWrapper.EventNewsRepository.ExistsAsync(
            eventNews => eventNews.Categories.Any(entity => entity.Id == request.Id));

        if (isInUse)
        {
            return Result.Fail<long>(EventNewsCategoryConstants.CantDeleteCategoryWhileAssociatedWithEventNews);
        }

        _repositoryWrapper.EventNewsCategoryRepository.Delete(category);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(category.Id)
                : Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsCategory)));
        }
        catch (DbUpdateException exception) when (exception.IsForeignKeyConstraintException())
        {
            return Result.Fail<long>(EventNewsCategoryConstants.CantDeleteCategoryWhileAssociatedWithEventNews);
        }
    }
}
