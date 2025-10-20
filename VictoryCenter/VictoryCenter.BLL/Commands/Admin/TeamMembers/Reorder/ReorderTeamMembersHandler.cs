using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : BaseHandler<ReorderTeamMembersCommand, Unit>
{
    private readonly IValidator<ReorderTeamMembersCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public ReorderTeamMembersHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<ReorderTeamMembersCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public override async Task<Unit> HandleRequest(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        using (var scope = _repositoryWrapper.BeginTransaction())
        {
            var orderedIds = request.ReorderTeamMembersDto.OrderedIds;
            var categoryId = request.ReorderTeamMembersDto.CategoryId;

            await _reorderService.SwapElementsAsync<TeamMember>(
                orderedIds,
                tm => tm.Id,
                tm => tm.CategoryId == categoryId);

            await _repositoryWrapper.SaveChangesAsync();

            scope.Complete();

            return Unit.Value;
        }
    }
}
