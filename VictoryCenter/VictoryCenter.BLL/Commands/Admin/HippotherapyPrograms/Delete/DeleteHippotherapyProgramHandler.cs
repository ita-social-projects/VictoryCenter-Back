using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Delete;

public class DeleteHippotherapyProgramHandler : BaseHandler<DeleteHippotherapyProgramCommand, long>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyProgramHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public override async Task<long> HandleRequest(DeleteHippotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        HippotherapyProgram? entityToDelete = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program.Include(p => p.Categories)
        });

        if (entityToDelete is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgram)));
        }

        entityToDelete.Categories.Clear();
        _repositoryWrapper.HippotherapyProgramsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() < 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgram)));
        }

        return entityToDelete.Id;
    }
}
