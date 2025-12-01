using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Partners.ReorderPartners;

public class ReorderPartnersHandler : IRequestHandler<ReorderPartnersCommand, Result<Unit>>
{
    private readonly IValidator<ReorderPartnersCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderPartnersHandler(
        IValidator<ReorderPartnersCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderPartnersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderDto.OrderedIds;
            var sectionId = request.ReorderDto.PartnersSectionId;

            await _reorderService.SwapElementsAsync<Partner>(
                idsOrder: orderedIds,
                idSelector: e => e.Id,
                groupSelector: e => e.PartnersSectionId == sectionId);

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Partner)));
        }
    }
}
