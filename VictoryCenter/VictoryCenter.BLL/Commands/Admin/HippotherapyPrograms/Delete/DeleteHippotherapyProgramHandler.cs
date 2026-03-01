using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Delete;

public class DeleteHippotherapyProgramHandler : IRequestHandler<DeleteHippotherapyProgramCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyProgramHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteHippotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.Sections)
                .ThenInclude(s => s.Contents)
        });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(HippotherapyProgram)));
        }

        var faqQuestions = entityToDelete.Sections
            .SelectMany(s => s.Contents)
            .OfType<FaqQuestionProgramContent>()
            .Select(c => c.FaqQuestion)
            .Where(q => q is not null)
            .ToList();

        if (faqQuestions.Count > 0)
        {
            _repositoryWrapper.FaqQuestionsRepository.DeleteRange(faqQuestions!);
        }

        entityToDelete.Categories.Clear();
        _repositoryWrapper.HippotherapyProgramsRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgram)));
    }
}
