using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Delete;

public class DeleteTeamCategoryHandler : IRequestHandler<DeleteTeamCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete =
            await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query.Include(x => x.TeamMembers)
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamCategory)));
        }

        if (entityToDelete.TeamMembers.Count != 0)
        {
            return Result.Fail<long>(TeamCategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyTeamMember);
        }

        _repositoryWrapper.TeamCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamCategory)));
    }
}
