using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsCategoryLink = VictoryCenter.DAL.Entities.EventNewsEventNewsCategories;
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

            var existingLinks = await repository
                .EventNewsEventNewsCategoriesRepository
                .GetAllAsync(new QueryOptions<EventNewsCategoryLink>
                {
                    Filter = e => e.CategoriesId == categoryId && orderedIds.Contains(e.EventsNewsId)
                });

            if (existingLinks is null)
            {
                return Result.Fail(ErrorMessagesConstants.NotFound(categoryId, typeof(EventNewsCategoryLink)));
            }

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventNewsEntity)));
        }
    }
}
