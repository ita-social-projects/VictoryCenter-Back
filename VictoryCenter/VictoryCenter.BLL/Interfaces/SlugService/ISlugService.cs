using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Interfaces.SlugService;

public interface ISlugService
{
    string GenerateSlug(string source);

    Task<string> GenerateUniqueHippotherapyProgramSlugAsync(long id, string programName, CancellationToken cancellationToken = default);

    Task<HippotherapyProgram?> GetHippotherapyProgramBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
