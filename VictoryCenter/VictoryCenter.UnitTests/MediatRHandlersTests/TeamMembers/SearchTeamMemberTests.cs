using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.TeamMembers.Search;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class SearchTeamMemberTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISearchService<TeamMember>> _searchServiceMock;
    private readonly IValidator<SearchTeamMemberQuery> _validator;

    private readonly List<TeamMember> _teamMembers = [
        new TeamMember
        {
            Id = 1,
            FullName = "TestName",
            Priority = 1,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
    ];

    private readonly List<TeamMemberDto> _teamMemberDtos = [
        new TeamMemberDto
        {
            Id = 1,
            FullName = "TestName",
            Priority = 1,
            CategoryName = "Name",
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com"
        },
    ];

    public SearchTeamMemberTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new SearchTeamMemberValidator();
        _searchServiceMock = new Mock<ISearchService<TeamMember>>();
    }

    [Fact]
    public async Task Handle_ExistingFullName_ShouldReturnNotEmpty()
    {
        // Arrange
        SetupMapper(_teamMemberDtos);
        SetupRepositoryWrapper(_teamMembers);
        var dto = new SearchTeamMemberDto { FullName = "TestName" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_teamMemberDtos, result.Value);
    }

    [Fact]
    public async Task Handle_NonexistentFullName_ShouldReturnEmpty()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchTeamMemberDto { FullName = "Nonexistent fullname" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_InvalidFullName_ShouldReturnValidationError()
    {
        // Arrange
        SetupMapper([]);
        SetupRepositoryWrapper([]);
        var dto = new SearchTeamMemberDto { FullName = "" };
        var query = new SearchTeamMemberQuery(dto);
        var handler = new SearchTeamMemberHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator, _searchServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("FullName field is required", result.Errors[0].Message);
    }

    private void SetupMapper(List<TeamMemberDto> teamMemberDtos)
    {
        _mapperMock.Setup(mapper => mapper.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>())).Returns(teamMemberDtos);
    }

    private void SetupRepositoryWrapper(List<TeamMember> membersToReturn)
    {
        _repositoryWrapperMock.Setup(x => x.TeamMembersRepository.GetAllAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(membersToReturn);
    }
}
