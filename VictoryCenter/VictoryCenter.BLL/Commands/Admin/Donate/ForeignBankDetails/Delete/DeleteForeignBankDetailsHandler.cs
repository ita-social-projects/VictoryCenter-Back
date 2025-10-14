using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Delete;
public class DeleteForeignBankDetailsHandler : IRequestHandler<DeleteForeignBankDetailsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteForeignBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Entities.ForeignBankDetails? entityToDelete = await _repositoryWrapper.ForeignBankDetailsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
                {
                    Filter = foreignBankDetails => foreignBankDetails.Id == request.Id
                });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.ForeignBankDetails)));
            }

            _repositoryWrapper.ForeignBankDetailsRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }

            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.ForeignBankDetails)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(Entities.ForeignBankDetails)));
        }
    }
}
