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

namespace VictoryCenter.BLL.Commands.Admin.Partners.Reorder;

public class ReorderPartnersSectionsHandler : IRequestHandler<ReorderPartnersSectionsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderPartnersSectionsCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public ReorderPartnersSectionsHandler(
        IValidator<ReorderPartnersSectionsCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderPartnersSectionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderDto.OrderedIds;

            var sectionsToReorderCount = await _repositoryWrapper.PartnerSectionsRepository.CountAsync(
                new QueryOptions<PartnerSection>
                {
                    Filter = e => orderedIds.Contains(e.Id),
                });

            if (sectionsToReorderCount == 0)
            {
                return Result.Fail<Unit>(PartnerConstants.HaveNotFoundAnyPartnersForReorder);
            }

            using var transactionScope = _repositoryWrapper.BeginTransaction();

            await _reorderService.SwapElementsAsync<PartnerSection>(
                idsOrder: orderedIds,
                idSelector: e => e.Id);

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSection)));
        }
    }
}
