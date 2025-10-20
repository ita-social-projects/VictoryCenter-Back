using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamMembers.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class DeleteTeamMemberTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IReorderService> _mockReorderService;

    private readonly TeamMember _testExistingTeamMember = new()
    {
        Id = 1,
        FullName = "Test name",
        CategoryId = 1,
        Priority = 1,
        Status = Status.Published,
        Email = "email@test.com",
        Description = "Test description",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
    };

    public DeleteTeamMemberTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
    }

    [Fact]
    public async Task Handle_TeamMemberExists_ShouldDeleteTeamMember()
    {
        SetupRepositoryWrapper(_testExistingTeamMember);
        SetupReorderService();

        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object, _mockReorderService.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(_testExistingTeamMember.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_testExistingTeamMember.Id, result.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_TeamMemberNotFound_ShouldReturnFailure(long teamMemberId)
    {
        SetupRepositoryWrapper();
        SetupReorderService();

        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object, _mockReorderService.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(teamMemberId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(teamMemberId, typeof(TeamMember)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        SetupRepositoryWrapper(_testExistingTeamMember, -1);
        SetupReorderService();

        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object, _mockReorderService.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(_testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMember)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionThrown_ShouldReturnFailure()
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(_testExistingTeamMember);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Database error"));

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        SetupReorderService();

        var handler = new DeleteTeamMemberHandler(_mockRepositoryWrapper.Object, _mockReorderService.Object);

        var result = await handler.Handle(new DeleteTeamMemberCommand(_testExistingTeamMember.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMember)), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(r => r.RenumberPriorityAsync<TeamMember>(It.IsAny<Expression<Func<TeamMember, bool>>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupRepositoryWrapper(TeamMember? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }
}
