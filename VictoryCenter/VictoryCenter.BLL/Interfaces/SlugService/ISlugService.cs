namespace VictoryCenter.BLL.Interfaces.SlugService;

public interface ISlugService
{
    string GenerateSlug(string source);

    Task<string> GenerateUniqueHippotherapyProgramSlugAsync(long id, string programName, CancellationToken cancellationToken = default);
}
