using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.BLL.Queries.Admin.ProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ProgramCategories;

public class GetProgramCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<ProgramCategory> _testProgramCategories =
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

    private readonly IEnumerable<ProgramCategoryDto> _testProgramCategoriesDtos =
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

    public GetProgramCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProgramCategories()
    {
        SetupDependencies();

        var handler = new GetProgramCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
        Result<List<ProgramCategoryDto>> result = await handler.Handle(new GetProgramCategoriesQuery(), CancellationToken.None);

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
        _mockMapper.Setup(x => x.Map<IEnumerable<ProgramCategoryDto>>(It.IsAny<IEnumerable<ProgramCategory>>()))
            .Returns(_testProgramCategoriesDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.ProgramCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<ProgramCategory>>()))
            .ReturnsAsync(_testProgramCategories);
    }
}
