using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;

public class ReorderFaqQuestionsHandler : IRequestHandler<ReorderFaqQuestionsCommand, Result<Unit>>
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

    public async Task<Result<Unit>> Handle(ReorderFaqQuestionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderFaqQuestionsDto.OrderedIds;
            var pageId = request.ReorderFaqQuestionsDto.PageId;

            var questionsToReorderCount = await _repositoryWrapper.FaqPlacementsRepository.CountAsync(
                new QueryOptions<FaqPlacement>
                {
                    Filter = e => e.PageId == pageId && orderedIds.Contains(e.QuestionId),
                    OrderByASC = e => e.Priority
                });

            if (questionsToReorderCount == 0)
            {
                return Result.Fail<Unit>(FaqConstants.PageNotFoundOrContainsNoFaqQuestions);
            }

            using var transactionScope = _repositoryWrapper.BeginTransaction();

            await _reorderService.SwapElementsAsync<FaqPlacement>(
                idsOrder: orderedIds,
                idSelector: e => e.QuestionId,
                groupSelector: e => e.PageId == pageId);

            transactionScope.Complete();
            return Result.Ok();
        }
        catch (ValidationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FaqQuestion)));
        }
    }
}
