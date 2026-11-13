using System.Globalization;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Interfaces.Audit;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;

public class DeleteVideoReviewHandler : IRequestHandler<DeleteVideoReviewCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private readonly IReorderService _reorderService;
    private readonly IAuditLogFactory _auditLogFactory;

    public DeleteVideoReviewHandler(
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider,
        IReorderService reorderService,
        IAuditLogFactory auditLogFactory)
    {
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
        _reorderService = reorderService;
        _auditLogFactory = auditLogFactory;
    }

    public async Task<Result<long>> Handle(DeleteVideoReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = await _repositoryWrapper.BeginTransactionAsync(cancellationToken);

            var archivedAt = _timeProvider.GetUtcNow();
            var archivedRows = await _repositoryWrapper.VideoReviewsRepository.ArchiveAsync(request.Id, archivedAt);

            if (archivedRows == 0)
            {
                var entity = await _repositoryWrapper.VideoReviewsRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<VideoReview>
                    {
                        Filter = videoReview => videoReview.Id == request.Id,
                        AsNoTracking = true
                    });

                if (entity is null)
                {
                    return Result.Fail<long>(new NotFoundError(ErrorMessagesConstants.NotFound(request.Id, typeof(VideoReview))));
                }

                return Result.Ok(entity.Id);
            }

            _repositoryWrapper.AddAuditLog(_auditLogFactory.Create(
                nameof(VideoReview),
                request.Id.ToString(CultureInfo.InvariantCulture),
                AuditOperation.Delete,
                new Dictionary<string, AuditChange>
                {
                    [nameof(VideoReview.IsArchived)] = new AuditChange(false, true),
                    [nameof(VideoReview.ArchivedAt)] = new AuditChange(null, archivedAt),
                }));
            await _repositoryWrapper.SaveChangesAsync();

            await _reorderService.RenumberPriorityAsync<VideoReview>(videoReview => !videoReview.IsArchived);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(request.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(VideoReview)));
        }
    }
}
