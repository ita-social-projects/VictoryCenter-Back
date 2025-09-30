using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Public.HypotherapyPrograms.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyPrograms;

public class GetPublishedHypotherapyProgramsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly List<DAL.Entities.HypotherapyProgram> _programEntities =
    [
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
        },
    ];

    private readonly IEnumerable<PublishedHypotherapyProgramDto> _programDto =
    [
        new()
        {
            Name = "TestName1",
            Description = "TestDescription1",
            Image = new ImageDto()
        },
        new()
        {
            Name = "TestName2",
            Description = "TestDescription2",
            Image = new ImageDto()
        },
    ];

    public GetPublishedHypotherapyProgramsTests()
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
        Result<List<PublishedHypotherapyProgramDto>> result = await handler.Handle(new GetPublishedProgramsQuery(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    private void SetUpDependencies(IEnumerable<DAL.Entities.HypotherapyProgram> programs = null!)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(programs);
        SetUpBlobService();
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<IEnumerable<PublishedHypotherapyProgramDto>>(It.IsAny<IEnumerable<DAL.Entities.HypotherapyProgram>>()))
            .Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(IEnumerable<DAL.Entities.HypotherapyProgram> programs)
    {
        _mockRepositoryWrapper.Setup(x => x.HypotherapyProgramsRepository
            .GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.HypotherapyProgram>>())).ReturnsAsync(programs);
    }

    private void SetUpBlobService()
    {
        _mockBlobService
            .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://localhost:5000/supersecretimage.png");
    }
}
