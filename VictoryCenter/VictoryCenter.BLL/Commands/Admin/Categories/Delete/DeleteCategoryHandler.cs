using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Delete;

public class DeleteCategoryHandler : BaseHandler<DeleteCategoryCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete =
            await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<Category>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query.Include(x => x.TeamMembers)
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(Category)));
        }

        if (entityToDelete.TeamMembers.Count != 0)
        {
            throw new Exception(CategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyTeamMember);
        }

        _repositoryWrapper.CategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Category)));
        }

        return entityToDelete.Id;
    }
}
