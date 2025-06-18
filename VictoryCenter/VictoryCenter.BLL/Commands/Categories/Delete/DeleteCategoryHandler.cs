using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Categories.Delete;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete =
            await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<Category>
            {
                FilterPredicate = entity => entity.Id == request.Id,
                Include = query => query.Include(x => x.TeamMembers)
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>("Not found");
        }

        if (entityToDelete.TeamMembers.Count != 0)
        {
            return Result.Fail<long>("Can't delete category while assotiated with any team member");
        }

        _repositoryWrapper.CategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>("Failed to delete category");
    }
}
