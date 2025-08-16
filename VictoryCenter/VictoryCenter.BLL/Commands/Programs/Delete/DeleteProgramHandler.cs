using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Programs.Delete;

public class DeleteProgramHandler : IRequestHandler<DeleteProgramCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteProgramHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
    {
        Program? entityToDelete = await _repositoryWrapper.ProgramsRepository.GetFirstOrDefaultAsync(new QueryOptions<Program>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program.Include(p => p.Categories)
        });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Program)));
        }

        entityToDelete.Categories.Clear();
        _repositoryWrapper.ProgramsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ProgramConstants.FailedToDeleteProgram);
    }
}
