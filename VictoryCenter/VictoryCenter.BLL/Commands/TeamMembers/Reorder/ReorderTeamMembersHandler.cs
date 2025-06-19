using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

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

            var allCategoryMembers = await GetAllMembersInCategoryAsync(categoryId);

            if (!allCategoryMembers.Any())
                return Result.Fail<Unit>("Category not found or contains no team members");

            // Ensure all provided IDs exist within the selected category
            var notFoundIds = orderedIds.Except(allCategoryMembers.Select(m => m.Id)).ToList();
            if (notFoundIds.Any())
                return Result.Fail<Unit>($"Invalid member IDs found: {string.Join(", ", notFoundIds)}");

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

            // Assign subsequent priority values to the remaining members
            var nextPosition = orderedIds.Count;
            foreach (var member in unchangedMembers.OrderBy(m => m.Priority)) // Preserving original order
            {
                member.Priority = nextPosition++;
            }

            await _repositoryWrapper.SaveChangesAsync();

            return Result.Ok();
        }
        catch (ValidationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
    }


    private async Task<List<TeamMember>> GetAllMembersInCategoryAsync(long categoryId)
    {
        var queryOptions = new QueryOptions<TeamMember>
        {
            Filter = x => x.CategoryId == categoryId,
        };

        return (await _repositoryWrapper.TeamMembersRepository.GetAllAsync(queryOptions)).ToList();
    }
}
