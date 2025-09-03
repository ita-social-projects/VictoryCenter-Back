using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Delete;

public class DeleteProgramCategoryHandler : IRequestHandler<DeleteProgramCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        ProgramCategory? entityToDelete = await _repositoryWrapper.ProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.id,
                Include = programCategory => programCategory
                    .Include(p => p.Programs)
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.id, typeof(ProgramCategory)));
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

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(ProgramCategory)));
    }
}
