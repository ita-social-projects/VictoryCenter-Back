using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Queries.Admin.TeamCategories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamCategories;

public class GetTeamCategoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<TeamCategory> _testCategoryEntities = new List<TeamCategory>
    {
        new ()
        {
            Id = 1,
            Name = "Test1",
            Description = "Test description1",
            TeamMembers =
            [
                new() { FullName = "Member1" },
                new() { FullName = "Member2" },
            ]
        },
        new()
        {
            Id = 2,
            Name = "Test2",
            Description = "Test description2",
            TeamMembers =
            [
                new() { FullName = "Member3" },
            ]
        },
    };
    private readonly IEnumerable<TeamCategoryDto> _testCategoryDtos = new List<TeamCategoryDto>
    {
        new()
        {
            Name = "Test1",
            Description = "Test description1",
            TeamMembersCount = 2,
        },
        new()
        {
            Name = "Test2",
            Description = "Test description2",
            TeamMembersCount = 1,
        },
    };

    public GetTeamCategoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        SetupDependencies();
        var handler = new GetAllTeamCategoriesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetAllTeamCategoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_testCategoryDtos.Count(), result.Value.Count());
        for (int i = 0; i < _testCategoryDtos.Count(); i++)
        {
            Assert.Equal(_testCategoryDtos.ElementAt(i).Name, result.Value.ElementAt(i).Name);
            Assert.Equal(_testCategoryDtos.ElementAt(i).Description, result.Value.ElementAt(i).Description);
            Assert.Equal(_testCategoryDtos.ElementAt(i).TeamMembersCount, result.Value.ElementAt(i).TeamMembersCount);
        }
    }

    private void SetupDependencies()
    {
        SetupMapper();
        SetupRepositoryWrapper();
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(x => x.Map<IEnumerable<TeamCategoryDto>>(It.IsAny<IEnumerable<TeamCategory>>()))
            .Returns(_testCategoryDtos);
    }

    private void SetupRepositoryWrapper()
    {
        _mockRepositoryWrapper.Setup(repo => repo.TeamCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<TeamCategory>>()))
                .ReturnsAsync(_testCategoryEntities);
    }
}
