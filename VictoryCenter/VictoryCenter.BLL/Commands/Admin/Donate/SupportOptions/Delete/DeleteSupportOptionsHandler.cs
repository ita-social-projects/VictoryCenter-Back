using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
public class DeleteSupportOptionsHandler : BaseHandler<DeleteSupportOptionsCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteSupportOptionsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        Entities.SupportOptions? entityToDelete = await _repositoryWrapper.SupportOptionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.SupportOptions>
            {
                Filter = supportOptions => supportOptions.Id == request.Id
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.SupportOptions)));
        }

        _repositoryWrapper.SupportOptionsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return entityToDelete.Id;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.SupportOptions)));
    }
}
