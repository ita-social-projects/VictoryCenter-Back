using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Delete;
public class DeleteTeamMemberHandler : BaseHandler<DeleteTeamMemberCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(
        DeleteTeamMemberCommand request,
        CancellationToken cancellationToken)
    {
        var entityToDelete = await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(
            new QueryOptions<TeamMember>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query.Include(tm => tm.Category)
            });

        if (entityToDelete is null)
        {
            throw new Exception(
                ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
        }

        entityToDelete.Category = null!;

        _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

        var changes = await _repositoryWrapper.SaveChangesAsync();
        if (changes <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMember)));
        }

        return entityToDelete.Id;
    }
}
