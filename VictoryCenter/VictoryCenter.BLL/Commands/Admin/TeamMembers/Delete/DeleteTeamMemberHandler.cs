using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Delete;
public class DeleteTeamMemberHandler : BaseHandler<DeleteTeamMemberCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper, IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
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

            using (var transactionScope = _repositoryWrapper.BeginTransaction())
            {
                var categoryId = entityToDelete.CategoryId;

                _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

        var changes = await _repositoryWrapper.SaveChangesAsync();
        if (changes <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMember)));
        }

        return entityToDelete.Id;
    }
}
