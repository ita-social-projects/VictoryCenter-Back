using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public class ReorderTeamMembersHandler : IRequestHandler<ReorderTeamMembersCommand, Result<Unit>>
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

    public async Task<Result<Unit>> Handle(ReorderTeamMembersCommand request, CancellationToken cancellationToken)
    {
        try
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

                return Result.Ok(Unit.Value);
            }
        }
        catch (ValidationException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamMember)));
        }
        catch(DbUpdateException ex)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
        }
    }
}
