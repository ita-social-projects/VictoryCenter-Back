using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;

public class ReorderFaqQuestionsHandler : BaseHandler<ReorderFaqQuestionsCommand, Unit>
{
    private readonly IValidator<ReorderFaqQuestionsCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public ReorderFaqQuestionsHandler(
        IValidator<ReorderFaqQuestionsCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public override async Task<Unit> HandleRequest(ReorderFaqQuestionsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var orderedIds = request.ReorderFaqQuestionsDto.OrderedIds;
        var pageId = request.ReorderFaqQuestionsDto.PageId;
        var duplicateIds = orderedIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateIds.Count > 0)
        {
            throw new Exception(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(request.ReorderFaqQuestionsDto.OrderedIds)));
        }

        var questionsToReorder = (await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
            new QueryOptions<FaqPlacement>
            {
                Filter = e => e.PageId == pageId && orderedIds.Contains(e.QuestionId),
                OrderByASC = e => e.Priority
            })).ToList();

        if (questionsToReorder.Count == 0)
        {
            throw new Exception(FaqConstants.PageNotFoundOrContainsNoFaqQuestions);
        }

        var notFoundIds = orderedIds.Except(questionsToReorder.Select(f => f.QuestionId));
        if (notFoundIds.Any())
        {
            throw new Exception(ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(FaqQuestion), notFoundIds));
        }

        var prioritiesFound = questionsToReorder.Select(q => q.Priority).OrderBy(p => p).ToList();
        for (var i = 1; i < prioritiesFound.Count; i++)
        {
            if (prioritiesFound[i] - prioritiesFound[i - 1] != 1)
            {
                throw new Exception(FaqConstants.IdsAreNonConsecutive);
            }
        }

        using var transactionScope = _repositoryWrapper.BeginTransaction();
        long minPriorityToSet = questionsToReorder.MinBy(e => e.Priority)!.Priority;

        // Temporarily assign negative priorities to avoid unique constraint conflicts during update
        foreach (var faq in questionsToReorder)
        {
            faq.Priority = -faq.Priority;
        }

        _repositoryWrapper.FaqPlacementsRepository.UpdateRange(questionsToReorder);
        await _repositoryWrapper.SaveChangesAsync();

        foreach (var questionId in orderedIds)
        {
            questionsToReorder.Single(e => e.QuestionId == questionId).Priority = minPriorityToSet++;
        }

        await _repositoryWrapper.SaveChangesAsync();
        transactionScope.Complete();

        return default;
    }
}
