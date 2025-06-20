using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : IRequestHandler<ReorderTeamMembersCommand, Result<Unit>>
{
    private readonly IValidator<ReorderTeamMembersCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderTeamMembersHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<ReorderTeamMembersCommand> validator)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderTeamMembersDto.OrderedIds;
            var categoryId = request.ReorderTeamMembersDto.CategoryId;

            var allCategoryMembers = await _repositoryWrapper.TeamMembersRepository.GetByCategoryIdAsync(categoryId);

            if (!allCategoryMembers.Any())
            {
                return Result.Fail<Unit>("Category not found or contains no team members");
            }

            // Ensure all provided IDs exist within the selected category
            var notFoundIds = orderedIds.Except(allCategoryMembers.Select(m => m.Id)).ToList();
            if (notFoundIds.Any())
            {
                return Result.Fail<Unit>($"Invalid member IDs found: {string.Join(", ", notFoundIds)}");
            }

            // Split members into two groups: those being reordered, and those left unchanged
            var reorderedMembers = allCategoryMembers.Where(m => orderedIds.Contains(m.Id)).ToList();
            var unchangedMembers = allCategoryMembers.Where(m => !orderedIds.Contains(m.Id)).ToList();

            // Assign new priority values to the reordered members based on their new positions
            for (int i = 0; i < orderedIds.Count; i++)
            {
                var memberId = orderedIds[i];
                var member = reorderedMembers.First(m => m.Id == memberId);
                member.Priority = i;
            }

            // Assign subsequent priority values to the remaining members, preserving original order
            var nextPosition = orderedIds.Count;
            foreach (var member in unchangedMembers.OrderBy(m => m.Priority))
            {
                member.Priority = nextPosition++;
            }

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok();
            }

            return Result.Fail<Unit>("Failed to update TeamMembers order");
        }
        catch (ValidationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
    }
}
