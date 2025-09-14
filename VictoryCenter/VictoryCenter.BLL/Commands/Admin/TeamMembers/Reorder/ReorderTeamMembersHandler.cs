using FluentResults;
using MediatR;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : IRequestHandler<ReorderTeamMembersCommand, Result<Unit>>
{
    /*    private readonly IValidator<ReorderTeamMembersCommand> _validator;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public ReorderTeamMembersHandler(
            IRepositoryWrapper repositoryWrapper,
            IValidator<ReorderTeamMembersCommand> validator)
        {
            _validator = validator;
            _repositoryWrapper = repositoryWrapper;
        }*/

    private readonly IReorderService _reorderService;

    public ReorderTeamMembersHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    /*    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken);

                using var transactionScope = _repositoryWrapper.BeginTransaction();

                var orderedIds = request.ReorderTeamMembersDto.OrderedIds;
                var categoryId = request.ReorderTeamMembersDto.CategoryId;

                var allCategoryMembers = (await _repositoryWrapper.TeamMembersRepository
                    .GetAllAsync(new QueryOptions<TeamMember>
                    {
                        Filter = x => x.CategoryId == categoryId
                    })).ToList();

                if (!allCategoryMembers.Any())
                {
                    return Result.Fail<Unit>(TeamMemberConstants.CategoryNotFoundOrContainsNoTeamMembers);
                }

                // Ensure all provided IDs exist within the selected category
                var notFoundIds = orderedIds.Except(allCategoryMembers.Select(m => m.Id)).ToList();
                if (notFoundIds.Any())
                {
                    return Result.Fail<Unit>(ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(TeamMember), notFoundIds));
                }

                // Split members into two groups: those being reordered, and those left unchanged
                var reorderedMembers = allCategoryMembers.Where(m => orderedIds.Contains(m.Id)).ToDictionary(x => x.Id, x => x);
                var unchangedMembers = allCategoryMembers.Where(m => !orderedIds.Contains(m.Id)).OrderBy(m => m.Priority).ToList();

                // Temporarily assign negative priorities to avoid unique constraint conflicts during update
                long tempPriority = -1;
                foreach (var member in allCategoryMembers)
                {
                    member.Priority = tempPriority--;
                    _repositoryWrapper.TeamMembersRepository.Update(member);
                }

                await _repositoryWrapper.SaveChangesAsync();

                // Assign new priority values to the reordered members based on their new positions
                for (var i = 0; i < orderedIds.Count; i++)
                {
                    var memberId = orderedIds[i];
                    reorderedMembers[memberId].Priority = i;
                }

                // Assign subsequent priority values to the remaining members, preserving original order
                var nextPosition = orderedIds.Count;
                foreach (var member in unchangedMembers)
                {
                    member.Priority = nextPosition++;
                }

                await _repositoryWrapper.SaveChangesAsync();

                transactionScope.Complete();

                return Result.Ok();
            }
            catch (ValidationException ex)
            {
                return Result.Fail(ex.Message);
            }
        }*/

    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        var result = await _reorderService.MoveElement<TeamMember, int>(
            request.TeamMemberId,
            request.AfterTeamMemberId,
            tm => tm.CategoryId == request.CategoryId
        );

        return result;
    }
}
