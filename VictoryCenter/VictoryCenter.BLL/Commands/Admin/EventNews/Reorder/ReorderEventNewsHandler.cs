using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using EventNewsCategoryLink = VictoryCenter.DAL.Entities.EventNewsEventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;

public class ReorderEventNewsHandler(
    IReorderService reorderService,
    IValidator<ReorderEventNewsCommand> validator)
    : IRequestHandler<ReorderEventNewsCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        ReorderEventNewsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.Dto.Ids;
            var categoryId = request.Dto.CategoryId;

            await reorderService.SwapElementsAsync<EventNewsCategoryLink>(
                orderedIds,
                e => e.EventsNewsId,
                e => e.CategoriesId == categoryId);

            return Result.Ok(Unit.Value);
        }
        catch (ValidationException e)
        {
            return Result.Fail(e.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamMember)));
        }
    }
}
