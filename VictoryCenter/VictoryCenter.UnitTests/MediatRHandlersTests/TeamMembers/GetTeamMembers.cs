using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.BLL.Queries.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    public async Task ShouldReturnSuccessfully_NoFilters(int pageNumber, int pageSize)
    {
        // Arrange
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Skip(pageNumber).Take(pageSize).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto 
        { 
            PageNumber
            = pageNumber,
            PageSize = pageSize,
            Status = null,
            CategoryName = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.Equal(teamMemberDtoList.Count, result.Value.Count),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task ShouldReturnSuccessfully_FilterByStatus()
    {
        // Arrange
        var status = Status.Draft;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Where(t => t.Status == status).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            PageNumber = 0,
            PageSize = 0,
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
    public async Task ShouldReturnSuccessfully_FilterByCategoryName()
    {
        // Arrange
        var category = new Category { Id = 2, Name = "Category 2" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Where(t => t.CategoryName == category.Name).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            PageNumber = 0,
            PageSize = 0,
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
    public async Task ShouldReturnSuccessfully_FilterByStatusAndCategoryName()
    {
        // Arrange
        var status = Status.Published;
        var category = new Category { Id = 1, Name = "Category 1" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Where(t => t.Status == status && t.CategoryName == category.Name).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            PageNumber = 0,
            PageSize = 0,
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
            new TeamMember()
            {
                Id = 1,
                Status = Status.Draft,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1" }
            },
            new TeamMember()
            {
                Id = 2,
                Status = Status.Draft,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Category 2" }
            },
            new TeamMember()
            {
                Id = 3,
                Status = Status.Published,
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
            new TeamMemberDto()
            {
                Id = 1,
                Status = Status.Draft,
                CategoryName = "Category 1"
            },
            new TeamMemberDto()
            {
                Id = 2,
                Status = Status.Draft,
                CategoryName = "Category 2"
            },
            new TeamMemberDto()
            {
                Id = 3,
                Status = Status.Published,
                CategoryName = "Category 1"
            },
        };

        return teamMemberDtoList;
    }


    private void SetupRepository(List<TeamMember> teamMembers)
    {
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository.GetAllAsync(
             It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>(),
             It.IsAny<Expression<Func<TeamMember, bool>>>(),
             It.IsAny<int>(),
             It.IsAny<int>(),
             It.IsAny<Expression<Func<TeamMember, object>>>(),
             It.IsAny<Expression<Func<TeamMember, object>>>(),
             It.IsAny<Expression<Func<TeamMember, TeamMember>>>())
        )
            .ReturnsAsync(teamMembers);
    }

    private void SetupMapper(List<TeamMemberDto> teamMemberDTOList)
    {
        _mockMapper
            .Setup(x => x.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>()))
            .Returns(teamMemberDTOList);
    }
}
