using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Delete;

public class DeleteProgramCategoryHandler : BaseHandler<DeleteProgramCategoryCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        ProgramCategory? entityToDelete = await _repositoryWrapper.ProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.Id,
                Include = programCategory => programCategory
                    .Include(p => p.Programs)
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(ProgramCategory)));
        }

        if (entityToDelete.Programs.Count != 0)
        {
            throw new Exception(ProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram);
        }

        _repositoryWrapper.ProgramCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(ProgramCategory)));
        }

        return entityToDelete.Id;
    }
}
