using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Delete;

public class DeleteHypotherapyProgramCategoryHandler : IRequestHandler<DeleteHypotherapyProgramCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHypotherapyProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteHypotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
    {
        HippotherapyProgramCategory? entityToDelete = await _repositoryWrapper.HypotherapyProgramCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramCategory>
            {
                Filter = programCategory => programCategory.Id == request.Id,
                Include = programCategory => programCategory
                    .Include(p => p.Programs)
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgramCategory)));
        }

        if (entityToDelete.Programs.Count != 0)
        {
            return Result.Fail(HypotherapyProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram);
        }

        _repositoryWrapper.HypotherapyProgramCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgramCategory)));
    }
}
