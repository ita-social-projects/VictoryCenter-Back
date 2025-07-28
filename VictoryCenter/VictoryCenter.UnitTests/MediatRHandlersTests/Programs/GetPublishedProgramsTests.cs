using Moq;
using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Programs.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class GetPublishedProgramsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly List<DAL.Entities.Program> _programEntities = new()
    {
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            ImageId = 1
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Published,
            ImageId = 2,
        }
    };

    private readonly IEnumerable<PublishedProgramDto> _programDto = new List<PublishedProgramDto>()
    {
        new()
        {
            Name = "TestName1",
            Description = "TestDescription1",
            Image = new ImageDTO()
        },
        new()
        {
            Name = "TestName2",
            Description = "TestDescription2",
            Image = new ImageDTO()
        }
    };

    public GetPublishedProgramsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ShouldFindPrograms()
    {
        SetUpDependencies(_programEntities);
        var handler = new GetPublishedProgramsHandler(_mapperMock.Object, _mockRepositoryWrapper.Object, _mockBlobService.Object);
        var result = await handler.Handle(new GetPublishedProgramsQuery(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    private void SetUpDependencies(List<DAL.Entities.Program> programs = null)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(programs);
        SetUpBlobService();
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<IEnumerable<PublishedProgramDto>>(It.IsAny<IEnumerable<DAL.Entities.Program>>()))
            .Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(List<DAL.Entities.Program> programs)
    {
        _mockRepositoryWrapper.Setup(x => x.ProgramsRepository
            .GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.Program>>())).ReturnsAsync(programs);
    }

    private void SetUpBlobService()
    {
        _mockBlobService
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }
}
