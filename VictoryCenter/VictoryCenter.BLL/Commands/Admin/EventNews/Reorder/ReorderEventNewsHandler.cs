using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;

public class ReorderEventNewsHandler(
    IReorderService reorderService,
    IValidator<ReorderEventNewsCommand> validator,
    IRepositoryWrapper repository)
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

            var existingEventNews = await repository
                .EventNewsRepository
                .GetAllAsync(new QueryOptions<EventNewsEntity>
                {
                    Filter = e => e.CategoryId == categoryId && orderedIds.Contains(e.Id)
                });

            if (existingEventNews.Count() != orderedIds.Count)
            {
                return Result.Fail(ErrorMessagesConstants.NotFound(categoryId, typeof(EventNewsEntity)));
            }

            await reorderService.SwapElementsAsync<EventNewsEntity>(
                orderedIds,
                e => e.Id,
                e => e.CategoryId == categoryId);

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventNewsEntity)));
        }
    }
}
