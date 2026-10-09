using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Delete;

public class DeleteEventNewsHandler : IRequestHandler<DeleteEventNewsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteEventNewsHandler(
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public async Task<Result<long>> Handle(
        DeleteEventNewsCommand request,
        CancellationToken cancellationToken)
    {
        var eventNews = await _repositoryWrapper.EventNewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query
                    .Include(entity => entity.Localizations),
                AsNoTracking = false,
                AsSplitQuery = true
            });

        if (eventNews is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
        }

        var categoryId = eventNews.CategoryId;

        await using var transaction = await _repositoryWrapper.BeginTransactionAsync(cancellationToken);

        _repositoryWrapper.EventNewsRepository.Delete(eventNews);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                return Result.Fail<long>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsEntity)));
            }

            await _reorderService.RenumberPriorityAsync<EventNewsEntity>(
                entity => entity.CategoryId == categoryId);

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(eventNews.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (!await _repositoryWrapper.EventNewsRepository.ExistsAsync(
                    entity => entity.Id == request.Id))
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
            }

            throw;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
