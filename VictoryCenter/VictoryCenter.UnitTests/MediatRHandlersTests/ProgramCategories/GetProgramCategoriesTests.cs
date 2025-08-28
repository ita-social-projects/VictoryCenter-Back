using AutoMapper;
using Moq;
using FluentResults;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ProgramCategories;

public class GetProgramCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IBlobService> _mockBlobService;

    private readonly IEnumerable<ProgramCategory> _testProgramCategories = new List<ProgramCategory>
    {
        new()
        {
            Id = 1,
            Name = "Test1"
        },
        new()
        {
            Id = 2,
            Name = "Test2"
        }
    };

    private readonly IEnumerable<ProgramCategoryDto> _testProgramCategoriesDtos = new List<ProgramCategoryDto>
    {
        new()
        {
            Name = "Test1"
        },
        new()
        {
            Name = "Test2"
        }
    };

    public GetProgramCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProgramCategories()
    {
        SetupDependencies();

        var handler = new GetProgramCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockBlobService.Object);
        Result<List<ProgramCategoryDto>> result = await handler.Handle(new GetProgramCategoriesQuery(), CancellationToken.None);

        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    private void SetupDependencies()
    {
        SetupMapper();
        SetupRepositoryWrapper();
        SetUpBlobService();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<IEnumerable<ProgramCategoryDto>>(It.IsAny<IEnumerable<ProgramCategory>>()))
            .Returns(_testProgramCategoriesDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.ProgramCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<ProgramCategory>>()))
            .ReturnsAsync(_testProgramCategories);
    }

    private void SetUpBlobService()
    {
        _mockBlobService
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }
}
