using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Delete;

public class DeleteFaqQuestionHandler : BaseHandler<DeleteFaqQuestionCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteFaqQuestionHandler(
        IRepositoryWrapper repositoryWrapper, IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public override async Task<long> HandleRequest(DeleteFaqQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToDelete = await _repositoryWrapper.FaqQuestionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FaqQuestion>
            {
                Filter = entity => entity.Id == request.Id,
                Include = e => e.Include(q => q.Placements),
            });

        if (questionToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestion)));
        }

        var placementsToDelete = questionToDelete.Placements.ToList();
        var removedPageIds = placementsToDelete.Select(fp => fp.PageId).Distinct().ToList();
        var affectedRows = 0;

        using var transactionScope = _repositoryWrapper.BeginTransaction();

        _repositoryWrapper.FaqPlacementsRepository.DeleteRange(placementsToDelete);
        affectedRows += await _repositoryWrapper.SaveChangesAsync();

        foreach (var pageId in removedPageIds)
        {
            await _reorderService.RenumberPriorityAsync<FaqPlacement>(
                groupSelector: fp => fp.PageId == pageId);

            affectedRows += await _repositoryWrapper.SaveChangesAsync();
        }

        _repositoryWrapper.FaqQuestionsRepository.Delete(questionToDelete);
        affectedRows += await _repositoryWrapper.SaveChangesAsync();

        if (affectedRows > 0)
        {
            transactionScope.Complete();
            return questionToDelete.Id;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FaqQuestion)));
    }
}
