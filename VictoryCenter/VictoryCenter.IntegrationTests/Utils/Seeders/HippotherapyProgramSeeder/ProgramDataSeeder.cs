using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slugify;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.HippotherapyProgramSeeder;

public class ProgramSeeder : BaseSeeder<DAL.Entities.HippotherapyProgram>
{
    private const int ProgramCount = 8;

    private readonly SlugHelper _slugHelper = new();

    public ProgramSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramSeeder> logger, IBlobService blobService)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ProgramsSeeder";
    public override int Order => (int)SeederExecutionOrder.HippotherapyPrograms;

    protected override async Task<List<DAL.Entities.HippotherapyProgram>> GenerateEntitiesAsync()
    {
        var programs = new List<DAL.Entities.HippotherapyProgram>();
        var categories = await DbContext.HippotherapyProgramCategories.Take(4).ToListAsync();
        for (var i = 0; i < ProgramCount; i++)
        {
            var selectedCategories = categories
                .OrderBy(_ => Guid.NewGuid())
                .Take(2)
                .ToList();

            var name = "TestName" + (i + 1);

            programs.Add(new()
            {
                Id = i + 1,
                Name = name,
                Slug = _slugHelper.GenerateSlug(name),
                Description = "TestDescription" + (i + 1),
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = DateTimeOffset.UtcNow,
                Categories = selectedCategories
            });
        }

        return programs;
    }
}
