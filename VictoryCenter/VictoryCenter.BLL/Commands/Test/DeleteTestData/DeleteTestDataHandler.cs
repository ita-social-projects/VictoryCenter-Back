using FluentResults;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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
            var entityToDelete = await _repositoryWrapper.TestRepository.GetFirstOrDefaultAsync(entity => entity.Id == request.Id);
            if (entityToDelete is null)
            {
                return Result.Fail("Entity not found");
            }
            
            _repositoryWrapper.TestRepository.Delete(entityToDelete);
            await _repositoryWrapper.SaveChangesAsync();
            return Result.Ok(entityToDelete.Id);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
