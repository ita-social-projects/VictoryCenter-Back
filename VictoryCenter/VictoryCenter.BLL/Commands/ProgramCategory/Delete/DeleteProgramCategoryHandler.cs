using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.ProgramCategory.Delete;

public class DeleteProgramCategoryHandler : IRequestHandler<DeleteProgramCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<DAL.Entities.ProgramCategory>
        {
            Filter = programCategory => programCategory.Id == request.id,
            Include = programCategory => programCategory
                .Include(p => p.Programs)
        };

        var entityToDelete = await _repositoryWrapper.ProgramCategoriesRepository
            .GetFirstOrDefaultAsync(queryOptions);

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.id, typeof(DAL.Entities.ProgramCategory)));
        }

        if (entityToDelete.Programs.Count != 0)
        {
            return Result.Fail(ProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram);
        }

        _repositoryWrapper.ProgramCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ProgramCategoryConstants.FailedToDeleteCategory);
    }
}
