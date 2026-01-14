using Moq;
using Slugify;
using VictoryCenter.BLL.Services.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class SlugServiceTests
{
    [Fact]
    public void GenerateSlug_ShouldDelegateToSlugHelper()
    {
        var slugHelperMock = new Mock<ISlugHelper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();

        slugHelperMock
            .Setup(s => s.GenerateSlug("Hello World"))
            .Returns("hello-world");

        var service = new SlugService(slugHelperMock.Object, repoWrapperMock.Object);

        var result = service.GenerateSlug("Hello World");

        Assert.Equal("hello-world", result);
        slugHelperMock.Verify(s => s.GenerateSlug("Hello World"), Times.Once);
    }

    [Fact]
    public async Task GenerateUniqueHippotherapyProgramSlugAsync_WhenNoSimilarSlugs_ShouldReturnBaseSlug()
    {
        var slugHelperMock = new Mock<ISlugHelper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var programsRepoMock = new Mock<IHippotherapyProgramsRepository>();

        repoWrapperMock.SetupGet(r => r.HippotherapyProgramsRepository).Returns(programsRepoMock.Object);

        slugHelperMock
            .Setup(s => s.GenerateSlug("My Program"))
            .Returns("my-program");

        programsRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(new List<HippotherapyProgram>
            {
                new() { Id = 2, Slug = "other" },
                new() { Id = 3, Slug = "another" },
            });

        var service = new SlugService(slugHelperMock.Object, repoWrapperMock.Object);

        var result = await service.GenerateUniqueHippotherapyProgramSlugAsync(1, "My Program");

        Assert.Equal("my-program", result);
    }

    [Fact]
    public async Task GenerateUniqueHippotherapyProgramSlugAsync_WhenSimilarSlugExists_ShouldReturnIncrementedSlug()
    {
        var slugHelperMock = new Mock<ISlugHelper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var programsRepoMock = new Mock<IHippotherapyProgramsRepository>();

        repoWrapperMock.SetupGet(r => r.HippotherapyProgramsRepository).Returns(programsRepoMock.Object);

        slugHelperMock
            .Setup(s => s.GenerateSlug("My Program"))
            .Returns("my-program");

        programsRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(new List<HippotherapyProgram>
            {
                new() { Id = 2, Slug = "my-program" },
                new() { Id = 3, Slug = "my-program-1" },
                new() { Id = 4, Slug = "my-program-2" },
            });

        var service = new SlugService(slugHelperMock.Object, repoWrapperMock.Object);

        var result = await service.GenerateUniqueHippotherapyProgramSlugAsync(1, "My Program");

        Assert.Equal("my-program-3", result);
    }

    [Fact]
    public async Task GetHippotherapyProgramBySlugAsync_ShouldCallRepositoryWithSlugFilter_AndReturnEntity()
    {
        var slugHelperMock = new Mock<ISlugHelper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var programsRepoMock = new Mock<IHippotherapyProgramsRepository>();

        repoWrapperMock.SetupGet(r => r.HippotherapyProgramsRepository).Returns(programsRepoMock.Object);

        var expected = new HippotherapyProgram { Id = 10, Slug = "test-slug" };

        QueryOptions<HippotherapyProgram>? capturedOptions = null;

        programsRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .Callback<QueryOptions<HippotherapyProgram>>(o => capturedOptions = o)
            .ReturnsAsync(expected);

        var service = new SlugService(slugHelperMock.Object, repoWrapperMock.Object);

        var result = await service.GetHippotherapyProgramBySlugAsync("test-slug");

        Assert.Same(expected, result);
        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions!.Filter);

        var compiledFilter = capturedOptions.Filter!.Compile();

        Assert.True(compiledFilter(new HippotherapyProgram { Slug = "test-slug" }));
        Assert.False(compiledFilter(new HippotherapyProgram { Slug = "another" }));

        Assert.NotNull(capturedOptions.Include);

        programsRepoMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()), Times.Once);
    }
}
