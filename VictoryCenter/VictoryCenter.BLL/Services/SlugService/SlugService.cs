using Slugify;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.SlugService;

public class SlugService : ISlugService
{
    private readonly ISlugHelper _slugHelper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public SlugService(ISlugHelper slugHelper, IRepositoryWrapper repositoryWrapper)
    {
        _slugHelper = slugHelper;
        _repositoryWrapper = repositoryWrapper;
    }

    public string GenerateSlug(string source)
        => _slugHelper.GenerateSlug(source);

    public async Task<string> GenerateUniqueHippotherapyProgramSlugAsync(long id, string programName, CancellationToken cancellationToken = default)
    {
        var baseSlug = GenerateSlug(programName);
        var currentSlug = baseSlug;
        var i = 1;

        var programs = await _repositoryWrapper.HippotherapyProgramsRepository.GetAllAsync(
            new QueryOptions<HippotherapyProgram>
            {
                AsNoTracking = true,
                Filter = p => p.Slug != null && p.Slug.StartsWith(baseSlug) && p.Id != id,
            });

        if (!programs.Any())
        {
            return baseSlug;
        }

        var existingSlugs = programs
            .Select(p => p.Slug)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        while (existingSlugs.Contains(currentSlug))
        {
            currentSlug = $"{baseSlug}-{i}";
            i++;
        }

        return currentSlug;
    }
}
