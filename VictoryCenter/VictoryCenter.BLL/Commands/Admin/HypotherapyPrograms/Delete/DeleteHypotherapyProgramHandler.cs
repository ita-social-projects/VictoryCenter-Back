using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Delete;

public class DeleteHypotherapyProgramHandler : IRequestHandler<DeleteHypotherapyProgramCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHypotherapyProgramHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteHypotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        HypotherapyProgram? entityToDelete = await _repositoryWrapper.HypotherapyProgramsRepository.GetFirstOrDefaultAsync(new QueryOptions<HypotherapyProgram>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program.Include(p => p.Categories)
        });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HypotherapyProgram)));
        }

        entityToDelete.Categories.Clear();
        _repositoryWrapper.HypotherapyProgramsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HypotherapyProgram)));
    }
}
