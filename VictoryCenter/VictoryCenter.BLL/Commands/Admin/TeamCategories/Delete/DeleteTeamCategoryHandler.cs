using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Delete;

public class DeleteTeamCategoryHandler : BaseHandler<DeleteTeamCategoryCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete =
            await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
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

        _repositoryWrapper.TeamCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Category)));
        }

        return entityToDelete.Id;
    }
}
