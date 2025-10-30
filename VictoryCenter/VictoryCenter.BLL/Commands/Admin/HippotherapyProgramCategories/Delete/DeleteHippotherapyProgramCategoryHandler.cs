using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;

public class DeleteHippotherapyProgramCategoryHandler : BaseHandler<DeleteHippotherapyProgramCategoryCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        HippotherapyProgramCategory? entityToDelete = await _repositoryWrapper.HippotherapyProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.Id,
                Include = programCategory => programCategory
                    .Include(p => p.Programs)
            });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgramCategory)));
        }

        if (entityToDelete.Programs.Count != 0)
        {
            throw new Exception(HippotherapyProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram);
        }

        _repositoryWrapper.HippotherapyProgramCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgramCategory)));
        }

        return entityToDelete.Id;
    }
}
