using FluentResults;
using MediatR;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.BLL.Commands.Test.DeleteTestData;

public class DeleteTestDataHandler : IRequestHandler<DeleteTestDataCommand, Result<int>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTestDataHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<int>> Handle(DeleteTestDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.TestRepository.GetFirstOrDefaultAsync(new QueryOptions<TestEntity>
            {
                FilterPredicate = entity => entity.Id == request.Id
            });

            if (entityToDelete is null)
            {
                return Result.Fail("Entity not found");
            }

            _repositoryWrapper.TestRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }

            return Result.Fail("Failed to delete test data");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
