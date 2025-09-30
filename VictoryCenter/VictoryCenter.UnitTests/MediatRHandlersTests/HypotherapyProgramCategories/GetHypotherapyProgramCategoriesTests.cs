using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.Queries.Admin.HypotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyProgramCategories;

public class GetHypotherapyProgramCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<HypotherapyProgramCategory> _testProgramCategories =
    [
        new()
        {
            Id = 1,
            Name = "Test1"
        },
        new()
        {
            Id = 2,
            Name = "Test2"
        },
    ];

    private readonly IEnumerable<HypotherapyProgramCategoryDto> _testProgramCategoriesDtos =
    [
        new()
        {
            Name = "Test1"
        },
        new()
        {
            Name = "Test2"
        },
    ];

    public GetHypotherapyProgramCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProgramCategories()
    {
        SetupDependencies();

        var handler = new GetHypotherapyProgramCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
        Result<List<HypotherapyProgramCategoryDto>> result = await handler.Handle(new GetHypotherapyProgramCategoriesQuery(), CancellationToken.None);

        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    private void SetupDependencies()
    {
        SetupMapper();
        SetupRepositoryWrapper();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<IEnumerable<HypotherapyProgramCategoryDto>>(It.IsAny<IEnumerable<HypotherapyProgramCategory>>()))
            .Returns(_testProgramCategoriesDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.HypotherapyProgramCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HypotherapyProgramCategory>>()))
            .ReturnsAsync(_testProgramCategories);
    }
}
