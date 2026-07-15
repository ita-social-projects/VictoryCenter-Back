using Microsoft.EntityFrameworkCore;
using Slugify;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

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
                Filter = p => p.Slug != null && p.Id != id,
            });

        var existingSlugs = programs
            .Select(p => p.Slug)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!)
            .Where(s => s.StartsWith(baseSlug, StringComparison.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (existingSlugs.Count == 0)
        {
            return baseSlug;
        }

        while (existingSlugs.Contains(currentSlug))
        {
            currentSlug = $"{baseSlug}-{i}";
            i++;
        }

        return currentSlug;
    }

    public async Task<string> GenerateUniqueEventNewsSlugAsync(long id, string title, CancellationToken cancellationToken = default)
    {
        var baseSlug = GenerateSlug(title);
        var currentSlug = baseSlug;
        var i = 1;

        var eventNewsItems = await _repositoryWrapper.EventNewsRepository.GetAllAsync(
            new QueryOptions<EventNewsEntity>
            {
                AsNoTracking = true,
                Filter = eventNews => eventNews.Slug != null && eventNews.Id != id,
            });

        var existingSlugs = eventNewsItems
            .Select(eventNews => eventNews.Slug)
            .Where(slug => !string.IsNullOrWhiteSpace(slug))
            .Select(slug => slug!)
            .Where(slug => slug.StartsWith(baseSlug, StringComparison.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (existingSlugs.Count == 0)
        {
            return baseSlug;
        }

        while (existingSlugs.Contains(currentSlug))
        {
            currentSlug = $"{baseSlug}-{i}";
            i++;
        }

        return currentSlug;
    }

    public Task<HippotherapyProgram?> GetHippotherapyProgramBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Slug == slug,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.PreviewImage)!
                .Include(p => p.BackgroundImage)!
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Contents)
                        .ThenInclude(c => c.Localizations)
                            .ThenInclude(l => l.Language)
                .Include(p => p.Localizations)
                    .ThenInclude(l => l.Language)
        };

        return _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(queryOptions);
    }
}
