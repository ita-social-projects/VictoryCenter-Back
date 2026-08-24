using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Delete;

public class DeleteEventNewsHandler : IRequestHandler<DeleteEventNewsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteEventNewsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
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
                    .Include(entity => entity.Categories)
                    .Include(entity => entity.Localizations),
                AsNoTracking = false,
                AsSplitQuery = true
            });

        if (eventNews is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
        }

        _repositoryWrapper.EventNewsRepository.Delete(eventNews);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(eventNews.Id)
                : Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsEntity)));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repositoryWrapper.EventNewsRepository.ExistsAsync(
                    entity => entity.Id == request.Id))
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
            }

            throw;
        }
    }
}
