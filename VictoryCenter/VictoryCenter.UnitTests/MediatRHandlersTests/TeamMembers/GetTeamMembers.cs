using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class GetTeamMembers
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    public GetTeamMembers()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 4)]
    [InlineData(0, 3)]
    [InlineData(1, 2)]
    public async Task Handle_ShouldReturnSuccessfully_NoFilters(int pageNumber, int pageSize)
    {
        // Arrange
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .OrderBy(t => t.Priority)
            .Skip(pageNumber)
            .Take(pageSize)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = pageNumber,
            Limit = pageSize,
            Status = null,
            CategoryName = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        var teamMemberDtoListOld = GetTeamMemberDtoList();
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEqual(teamMemberDtoListOld.Count, result.Value.Count),
            () => Assert.NotEqual(teamMemberDtoListOld, result.Value),
            () => Assert.Equal(teamMemberDtoList.Count, result.Value.Count),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByStatus()
    {
        // Arrange
        var status = Status.Draft;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Status == status)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            CategoryName = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByCategoryName()
    {
        // Arrange
        var category = new Category { Id = 2, Name = "Category 2" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.CategoryName == category.Name)
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = null,
            CategoryName = category.Name
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByStatusAndCategoryName()
    {
        // Arrange
        var status = Status.Published;
        var category = new Category { Id = 1, Name = "Category 1" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Status == status && t.CategoryName == category.Name)
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            CategoryName = category.Name
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    private static List<TeamMember> GetTeamMemberList()
    {
        var teamMemberList = new List<TeamMember>()
        {
            new()
            {
                Id = 4,
                Priority = 3,
                Status = Status.Draft,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1" }
            },
            new()
            {
                Id = 2,
                Priority = 2,
                Status = Status.Draft,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Category 2" }
            },
            new()
            {
                Id = 3,
                Priority = 1,
                Status = Status.Published,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1" }
            },
            new()
            {
                Id = 5,
                Priority = 2,
                Status = Status.Published,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Category 2" }
            },
            new()
            {
                Id = 1,
                Priority = 1,
                Status = Status.Draft,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1" }
            },
        };

        return teamMemberList;
    }

    private static List<TeamMemberDto> GetTeamMemberDtoList()
    {
        var teamMemberDtoList = new List<TeamMemberDto>()
        {
            new()
            {
                Id = 1,
                Priority = 1,
                Status = Status.Draft,
                CategoryName = "Category 1"
            },
            new()
            {
                Id = 3,
                Priority = 1,
                Status = Status.Published,
                CategoryName = "Category 1"
            },
            new()
            {
                Id = 2,
                Priority = 2,
                Status = Status.Draft,
                CategoryName = "Category 2"
            },
            new()
            {
                Id = 5,
                Priority = 2,
                Status = Status.Published,
                CategoryName = "Category 2"
            },
            new()
            {
                Id = 4,
                Priority = 3,
                Status = Status.Draft,
                CategoryName = "Category 1"
            },
        };

        return teamMemberDtoList;
    }

    private void SetupRepository(List<TeamMember> teamMembers)
    {
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository.GetAllAsync(
             It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMembers);
    }

    private void SetupMapper(List<TeamMemberDto> teamMemberDTOList)
    {
        _mockMapper
            .Setup(x => x.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>()))
            .Returns(teamMemberDTOList);
    }
}
