using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.BLL.Queries.TeamMembers.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class GetTeamMemberById
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    public GetTeamMemberById()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task ShouldReturnSuccessfully_ExistingId()
    {
        // Arrange
        var teamMember = GetTeamMember();
        var teamMemberDto = GetTeamMemberDto();

        SetupRepository(teamMember);
        SetupMapper(teamMemberDto);

        var handler = new GetTeamMemberByIdHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMemberByIdQuery(teamMember.Id), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.IsType<TeamMemberDto>(result.ValueOrDefault),
            () => Assert.Equal(result.Value.Id, teamMember.Id));
    }

    [Fact]
    public async Task ShouldReturnErrorResponse_NotExistingId()
    {
        // Arrange
        var teamMember = GetTeamMember();
        var expectedError = "Team member not fould";

        SetupRepository(GetTeamMemberWithNotExistingId());
        SetupMapper(GetTeamDTOWithNotExistingId());

        var handler = new GetTeamMemberByIdHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMemberByIdQuery(teamMember.Id), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.True(result.IsFailed),
            () => Assert.Equal(expectedError, result.Errors[0].Message));
    }

    private static TeamMember GetTeamMember()
    {
        return new TeamMember
        {
            Id = 1,
        };
    }

    private static TeamMember? GetTeamMemberWithNotExistingId()
    {
        return null;
    }

    private static TeamMemberDto GetTeamMemberDto()
    {
        return new TeamMemberDto
        {
            Id = 1,
        };
    }

    private static TeamMemberDto? GetTeamDTOWithNotExistingId()
    {
        return null;
    }

    private void SetupRepository(TeamMember? teamMember)
    {
        _mockRepository.Setup(x => x.TeamMembersRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(teamMember);
    }

    private void SetupMapper(TeamMemberDto? teamMemberDTO)
    {
        _mockMapper.Setup(m => m
            .Map<TeamMemberDto?>(It.IsAny<TeamMember>()))
            .Returns(teamMemberDTO);
    }
}
