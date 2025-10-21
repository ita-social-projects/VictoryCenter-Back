using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Delete;
public class DeleteForeignBankDetailsHandler : BaseHandler<DeleteForeignBankDetailsCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteForeignBankDetailsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        Entities.ForeignBankDetails? entityToDelete = await _repositoryWrapper.ForeignBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
            {
                Filter = foreignBankDetails => foreignBankDetails.Id == request.Id
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.ForeignBankDetails)));
        }

        _repositoryWrapper.ForeignBankDetailsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return entityToDelete.Id;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.ForeignBankDetails)));
    }
}
