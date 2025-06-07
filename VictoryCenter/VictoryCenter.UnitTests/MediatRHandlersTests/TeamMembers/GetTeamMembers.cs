using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
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
            CategoryId = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
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
            CategoryId = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task ShouldReturnSuccessfully_FilterByCategoryId()
    {
        // Arrange
        var categoryId = 2;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Where(t => t.CategoryId == categoryId).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            PageNumber = 0,
            PageSize = 0,
            Status = null,
            CategoryId = categoryId
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.Equal(teamMemberDtoList, result.Value));
    }

    [Fact]
    public async Task ShouldReturnSuccessfully_FilterByStatusAndCategoryId()
    {
        // Arrange
        var status = Status.Published;
        var categoryId = 1;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList().Where(t => t.Status == status && t.CategoryId == categoryId).ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            PageNumber = 0,
            PageSize = 0,
            Status = status,
            CategoryId = categoryId
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
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
            },
            new TeamMember()
            {
                Id = 2,
                Status = Status.Draft,
                CategoryId = 2,
            },
            new TeamMember()
            {
                Id = 3,
                Status = Status.Published,
                CategoryId = 1,
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
            },
            new TeamMemberDto()
            {
                Id = 2,
            },
            new TeamMemberDto()
            {
                Id = 3,
            },
        };

        return teamMemberDtoList;
    }


    private void SetupRepository(List<TeamMember> teamMembers)
    {
        _mockRepository.Setup(x => x.TeamMembersRepository
            .GetTeamMembersAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<long?>(),
                It.IsAny<Status?>()))
            .ReturnsAsync(teamMembers);
    }

    private void SetupMapper(List<TeamMemberDto> teamMemberDTOList)
    {
        _mockMapper
            .Setup(x => x.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>()))
            .Returns(teamMemberDTOList);
    }
}
