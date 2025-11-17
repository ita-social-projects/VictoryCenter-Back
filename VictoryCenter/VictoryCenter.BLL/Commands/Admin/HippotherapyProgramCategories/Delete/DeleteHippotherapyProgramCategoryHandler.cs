using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;

public class DeleteHippotherapyProgramCategoryHandler : IRequestHandler<DeleteHippotherapyProgramCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyProgramCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteHippotherapyProgramCategoryCommand request, CancellationToken cancellationToken)
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
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgramCategory)));
        }

        if (entityToDelete.Programs.Count != 0)
        {
            return Result.Fail(HippotherapyProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram);
        }

        _repositoryWrapper.HippotherapyProgramCategoriesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgramCategory)));
    }
}
