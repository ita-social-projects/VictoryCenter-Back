using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Delete;
public class DeleteCorrespondentBankDetailsHandler : IRequestHandler<DeleteCorrespondentBankDetailsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCorrespondentBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteCorrespondentBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Entities.CorrespondentBankDetails? entityToDelete = await _repositoryWrapper.CorrespondentBankDetailsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.CorrespondentBankDetails>
                {
                    Filter = correspondentBankDetails => correspondentBankDetails.Id == request.Id
                });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.CorrespondentBankDetails)));
            }

            _repositoryWrapper.CorrespondentBankDetailsRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }

            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.CorrespondentBankDetails)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(Entities.CorrespondentBankDetails)));
        }
    }
}
