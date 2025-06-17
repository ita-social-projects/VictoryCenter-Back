using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Queries.Categories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Categories;

public class GetCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Category> _testCategoryEntities = new List<Category>
    {
        new ()
        {
            Id = 1,
            Name = "Test1",
            Description = "Test description1",
        },
        new()
        {
            Id = 2,
            Name = "Test2",
            Description = "Test description2",
        },
    };
    private readonly IEnumerable<CategoryDto> _testCategoryDtos = new List<CategoryDto>
    {
        new()
        {
            Name = "Test1",
            Description = "Test description1",
        },
        new()
        {
            Name = "Test2",
            Description = "Test description2",
        },
    };

    public GetCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        SetupDependencies();
        var handler = new GetAllCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Value);
    }

    private void SetupDependencies()
    {
        SetupMapper();
        SetupRepositoryWrapper();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(_testCategoryDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.CategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(_testCategoryEntities);
    }
}
