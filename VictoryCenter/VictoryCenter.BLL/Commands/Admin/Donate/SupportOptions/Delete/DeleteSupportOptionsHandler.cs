using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
public class DeleteSupportOptionsHandler : IRequestHandler<DeleteSupportOptionsCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteSupportOptionsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        Entities.SupportOptions? entityToDelete = await _repositoryWrapper.SupportOptionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.SupportOptions>
            {
                Filter = supportOptions => supportOptions.Id == request.Id
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.SupportOptions)));
        }

        _repositoryWrapper.SupportOptionsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.SupportOptions)));
    }
}
