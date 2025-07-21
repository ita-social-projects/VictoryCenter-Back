using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;

public class ReorderFaqQuestionsHandler : IRequestHandler<ReorderFaqQuestionsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderFaqQuestionsCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderFaqQuestionsHandler(
        IValidator<ReorderFaqQuestionsCommand> validator,
        IRepositoryWrapper repositoryWrapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<Unit>> Handle(ReorderFaqQuestionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderFaqQuestionsDto.OrderedIds;
            var pageId = request.ReorderFaqQuestionsDto.PageId;

            var questionsToReorder = (await _repositoryWrapper.FaqPlacementsRepository.GetAllAsync(
                new QueryOptions<FaqPlacement>
                {
                    Filter = e => e.PageId == pageId && orderedIds.Contains(e.QuestionId),
                    OrderByASC = e => e.Priority
                })).ToList();

            if (!questionsToReorder.Any())
            {
                return Result.Fail<Unit>(FaqConstants.PageNotFoundOrContainsNoFaqQuestions);
            }

            var notFoundIds = orderedIds.Except(questionsToReorder.Select(f => f.QuestionId));
            if (notFoundIds.Any())
            {
                return Result.Fail<Unit>(ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(TeamMember), notFoundIds));
            }

            using var transactionScope = _repositoryWrapper.BeginTransaction();
            long minPriorityToSet = questionsToReorder.MinBy(e => e.Priority)!.Priority;

            // Temporarily assign negative priorities to avoid unique constraint conflicts during update
            long tempPriority = -1;
            foreach (var faq in questionsToReorder)
            {
                faq.Priority = tempPriority--;
            }

            _repositoryWrapper.FaqPlacementsRepository.UpdateRange(questionsToReorder);
            await _repositoryWrapper.SaveChangesAsync();

            foreach (var questionId in orderedIds)
            {
                questionsToReorder.Single(e => e.QuestionId == questionId).Priority = minPriorityToSet++;
            }

            await _repositoryWrapper.SaveChangesAsync();
            transactionScope.Complete();

            return Result.Ok();
        }
        catch (ValidationException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
