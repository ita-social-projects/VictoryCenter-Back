using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Delete;
public class DeleteCorrespondentBankDetailsHandler : BaseHandler<DeleteCorrespondentBankDetailsCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCorrespondentBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteCorrespondentBankDetailsCommand request, CancellationToken cancellationToken)
    {
        Entities.CorrespondentBankDetails? entityToDelete = await _repositoryWrapper.CorrespondentBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.CorrespondentBankDetails>
            {
                Filter = correspondentBankDetails => correspondentBankDetails.Id == request.Id
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.CorrespondentBankDetails)));
        }

        _repositoryWrapper.CorrespondentBankDetailsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return entityToDelete.Id;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.CorrespondentBankDetails)));
    }
}
