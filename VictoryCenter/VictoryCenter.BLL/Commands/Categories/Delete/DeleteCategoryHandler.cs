using FluentResults;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Categories.Delete;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<long>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                entity => entity.Id == request.Id);

            if (entityToDelete is null)
            {
                return Result.Fail<long>("Not found");
            }

            if (entityToDelete.TeamMembers.Count != 0)
            {
                return Result.Fail<long>("Can't delete category while assotiated with any team member");
            }
            
            _repositoryWrapper.CategoriesRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }
            return Result.Fail<long>("Failed to delete category");
        }
        catch (Exception ex)
        {
            return Result.Fail<long>(ex.Message);
        }
    }
}
