using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Delete;

public class DeleteFaqQuestionHandler : IRequestHandler<DeleteFaqQuestionCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFaqQuestionHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToDelete = await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FaqQuestion>
            {
                Filter = entity => entity.Id == request.Id,
                Include = e => e.Include(q => q.Placements),
            });

        if (questionToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestion)));
        }

        var placementsToDelete = questionToDelete.Placements.ToList();
        using var transactionScope = _repositoryWrapper.BeginTransaction();

        int affectedRows = 0;

        var removedPageIds = placementsToDelete.Select(fp => fp.PageId).ToList();
        var pageGroups = (await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
            new QueryOptions<FaqPlacement>
            {
                Filter = fp => removedPageIds.Contains(fp.PageId),
            }))
            .GroupBy(p => p.PageId).ToList();

        _repositoryWrapper.FaqPlacementsRepository.DeleteRange(placementsToDelete);
        affectedRows += await _repositoryWrapper.SaveChangesAsync();

        foreach (var id in removedPageIds)
        {
            var question = placementsToDelete.Single(q => q.PageId == id);
            var group = pageGroups
                .Single(g => g.Key == id)
                .OrderBy(q => q.Priority)
                .Skip((int)question.Priority)
                .ToList();

            foreach (var faq in group)
            {
                faq.Priority = -(faq.Priority - 1);
            }

            _repositoryWrapper.FaqPlacementsRepository.UpdateRange(group);
            affectedRows += await _repositoryWrapper.SaveChangesAsync();

            foreach (var faq in group)
            {
                faq.Priority = -faq.Priority;
            }
        }

        _repositoryWrapper.FaqQuestionsRepository.Delete(questionToDelete);
        affectedRows += await _repositoryWrapper.SaveChangesAsync();

        if (affectedRows > 0)
        {
            transactionScope.Complete();
            return Result.Ok(questionToDelete.Id);
        }

        return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FaqQuestion)));
    }
}
