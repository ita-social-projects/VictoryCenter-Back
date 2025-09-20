using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : IRequestHandler<ReorderTeamMembersCommand, Result<Unit>>
{
    private readonly IValidator<ReorderTeamMembersCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IIndexReorderService _indexReorderService;

    public ReorderTeamMembersHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<ReorderTeamMembersCommand> validator,
        IIndexReorderService indexReorderService)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _indexReorderService = indexReorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                var orderedIds = request.ReorderTeamMembersDto.OrderedIds.Distinct().ToList();
                var categoryId = request.ReorderTeamMembersDto.CategoryId;

                await _indexReorderService.SwapElements<TeamMember>(
                    orderedIds,
                    tm => tm.Id,
                    tm => tm.CategoryId == categoryId);

                await _repositoryWrapper.SaveChangesAsync();
                scope.Complete();

                return Result.Ok(Unit.Value);
            }
        }
        catch (ValidationException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Fail("An unexpected error occurred: " + ex.Message);
        }
    }
}

/*using FluentResults;
using MediatR;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : IRequestHandler<ReorderTeamMembersCommand, Result<Unit>>
{
    private readonly IReorderService _reorderService;

    public ReorderTeamMembersHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        await _reorderService.MoveElement<TeamMember>(
            request.ReorderTeamMembersDto.TeamMemberId,
            request.ReorderTeamMembersDto.AfterTeamMemberId,
            tm => tm.Id,
            tm => tm.CategoryId == request.ReorderTeamMembersDto.CategoryId);

        return Result.Ok();
    }
}
*/
