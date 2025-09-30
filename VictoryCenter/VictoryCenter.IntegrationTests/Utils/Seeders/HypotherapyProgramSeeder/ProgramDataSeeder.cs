using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.HypotherapyProgramSeeder;

public class ProgramSeeder : BaseSeeder<DAL.Entities.HypotherapyProgram>
{
    private const int ProgramCount = 8;

    public ProgramSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramSeeder> logger, IBlobService blobService)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ProgramsSeeder";
    public override int Order => (int)SeederExecutionOrder.Programs;

    protected override async Task<List<DAL.Entities.HypotherapyProgram>> GenerateEntitiesAsync()
    {
        var programs = new List<DAL.Entities.HypotherapyProgram>();
        var categories = await DbContext.ProgramCategories.Take(4).ToListAsync();
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
                CreatedAt = DateTimeOffset.UtcNow,
                Categories = selectedCategories
            });
        }

        return programs;
    }
}
