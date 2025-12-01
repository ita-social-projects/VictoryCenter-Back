using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Delete;

public class DeleteUahBankDetailsHandler : IRequestHandler<DeleteUahBankDetailsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteUahBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Entities.UahBankDetails? entityToDelete = await _repositoryWrapper.UahBankDetailsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.UahBankDetails>
                {
                    Filter = uahBankDetails => uahBankDetails.Id == request.Id
                });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.UahBankDetails)));
            }

            _repositoryWrapper.UahBankDetailsRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }

            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.UahBankDetails)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(Entities.UahBankDetails)));
        }
    }
}
