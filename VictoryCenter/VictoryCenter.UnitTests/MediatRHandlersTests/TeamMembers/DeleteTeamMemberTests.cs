using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Delete;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class DeleteTeamMemberTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly TeamMember _testExistingTeamMember = new ()
    {
        Id = 1,
        FirstName = "Test name",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Email = "email@test.com",
        Description = "Test description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    };

    public DeleteTeamMemberTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_TeamMemberExists_ShouldDeleteTeamMember()
    {
        SetupRepositoryWrapper(_testExistingTeamMember);
        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(_testExistingTeamMember.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_TeamMemberNotFound_ShouldReturnFailure(long teamMemberId)
    {
        SetupRepositoryWrapper();
        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(teamMemberId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal($"Team member with ID {teamMemberId} not found.", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        SetupRepositoryWrapper(_testExistingTeamMember, -1);
        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(_testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to delete team member.", result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(TeamMember? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
