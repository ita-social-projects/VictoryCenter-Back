using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class GetPublishedHippotherapyProgramsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly List<HippotherapyProgram> _programEntities =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Published
        }

    ];

    private readonly IEnumerable<PublishedHippotherapyProgramDto> _programDto =
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

    public GetPublishedHippotherapyProgramsTests()
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
        Result<List<PublishedHippotherapyProgramDto>> result = await handler.Handle(new GetPublishedProgramsQuery(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    private void SetUpDependencies(IEnumerable<HippotherapyProgram> programs = null!)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(programs);
        SetUpBlobService();
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<IEnumerable<PublishedHippotherapyProgramDto>>(It.IsAny<IEnumerable<HippotherapyProgram>>()))
            .Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(IEnumerable<HippotherapyProgram> programs)
    {
        _mockRepositoryWrapper.Setup(x => x.HippotherapyProgramsRepository
            .GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>())).ReturnsAsync(programs);
    }

    private void SetUpBlobService()
    {
        _mockBlobService
            .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://localhost:5000/supersecretimage.png");
    }
}
