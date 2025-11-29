using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Partners.ReorderSections;

public class ReorderPartnersSectionsHandler : IRequestHandler<ReorderPartnersSectionsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderPartnersSectionsCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderPartnersSectionsHandler(
        IValidator<ReorderPartnersSectionsCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderPartnersSectionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderDto.OrderedIds;

            await _reorderService.SwapElementsAsync<PartnerSection>(
                idsOrder: orderedIds,
                idSelector: e => e.Id);

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
