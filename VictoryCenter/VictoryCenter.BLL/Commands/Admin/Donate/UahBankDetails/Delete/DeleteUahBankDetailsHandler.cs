using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Delete;
public class DeleteUahBankDetailsHandler : BaseHandler<DeleteUahBankDetailsCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteUahBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        Entities.UahBankDetails? entityToDelete = await _repositoryWrapper.UahBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.UahBankDetails>
            {
                Filter = uahBankDetails => uahBankDetails.Id == request.Id
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.UahBankDetails)));
        }

        _repositoryWrapper.UahBankDetailsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return entityToDelete.Id;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.UahBankDetails)));
    }
}
