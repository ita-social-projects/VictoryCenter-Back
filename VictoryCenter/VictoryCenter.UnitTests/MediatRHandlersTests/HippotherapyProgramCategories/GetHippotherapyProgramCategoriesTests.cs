using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.Queries.Admin.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyProgramCategories;

public class GetHippotherapyProgramCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<HippotherapyProgramCategory> _testProgramCategories =
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

    private readonly IEnumerable<HippotherapyProgramCategoryDto> _testProgramCategoriesDtos =
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

    public GetHippotherapyProgramCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProgramCategories()
    {
        SetupDependencies();

        var handler = new GetHippotherapyProgramCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
        Result<List<HippotherapyProgramCategoryDto>> result = await handler.Handle(new GetHippotherapyProgramCategoriesQuery(), CancellationToken.None);

        Assert.NotEmpty(result.Value);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithInclude()
    {
        // Arrange
        SetupDependencies();
        var handler = new GetHippotherapyProgramCategoriesHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        await handler.Handle(new GetHippotherapyProgramCategoriesQuery(), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.HippotherapyProgramCategoriesRepository.GetAllAsync(
                It.Is<QueryOptions<HippotherapyProgramCategory>>(opts =>
                    opts.Include != null)),
            Times.Once);
    }

    private void SetupDependencies()
    {
        SetupMapper();
        SetupRepositoryWrapper();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<IEnumerable<HippotherapyProgramCategoryDto>>(It.IsAny<IEnumerable<HippotherapyProgramCategory>>()))
            .Returns(_testProgramCategoriesDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.HippotherapyProgramCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(_testProgramCategories);
    }
}
