using FluentResults;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Categories.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<int>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<int>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                entity => entity.Id == request.id);

            if (entityToDelete is null)
            {
                return Result.Fail<int>("Entity not found");
            }
            
            _repositoryWrapper.CategoriesRepository.Delete(entityToDelete);
            await _repositoryWrapper.SaveChangesAsync();
            
            return Result.Ok(entityToDelete.Id);
        }
        catch (Exception ex)
        {
            return Result.Fail<int>(ex.Message);
        }
    }
}