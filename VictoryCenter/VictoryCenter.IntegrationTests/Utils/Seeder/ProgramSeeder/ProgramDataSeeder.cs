using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using Microsoft.Extensions.Logging;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.ProgramSeeder;

public class ProgramSeeder : BaseSeeder<DAL.Entities.Program>
{
    private const int ProgramCount = 8;

    public ProgramSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramSeeder> logger, IBlobService blobService)
        : base(dbContext, logger, blobService)
    {
    }

    public override string Name => "ProgramsSeeder";
    public override int Order => 4;
    protected override Task<bool> ShouldSkipAsync()
    {
        return Task.FromResult(_dbContext.Programs.Any());
    }

    protected override async Task<List<DAL.Entities.Program>> GenerateEntitiesAsync()
    {
        var programs = new List<DAL.Entities.Program>();
        var categories = await _dbContext.ProgramCategories.Take(4).ToListAsync();
        for (var i = 0; i < ProgramCount; i++)
        {
            var selectedCategories = categories
                .OrderBy(_ => Guid.NewGuid())
                .Take(2)
                .ToList();
            programs.Add(new()
            {
                Id = i + 1,
                Name = "TestName" + (i + 1),
                Description = "TestDescription" + (i + 1),
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = DateTime.UtcNow,
                Categories = selectedCategories
            });
        }

        return programs;
    }
}
