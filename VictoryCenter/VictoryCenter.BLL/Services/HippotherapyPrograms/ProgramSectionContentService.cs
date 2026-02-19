using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyPrograms;

public class ProgramSectionContentService : IProgramSectionContentService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ProgramSectionContentService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Dictionary<long, ContentType>> GetContentTypesByProgramIdAsync(long programId, CancellationToken cancellationToken = default)
    {
        var program = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<HippotherapyProgram>
            {
                Filter = entity => entity.Id == programId,
                Include = query => query.Include(entity => entity.Sections)
                    .ThenInclude(section => section.Contents),
                AsNoTracking = true,
            });

        if (program is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgram)));
        }

        return program.Sections
            .SelectMany(section => section.Contents)
            .ToDictionary(content => content.Id, content => content.ContentType);
    }
}
