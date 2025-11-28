using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class DeleteTeamMemberLocalizationTests
{
    private readonly Mock<ILocalizationService<TeamMember, TeamMemberLocalization>> _mockLocalizationService;

    private readonly TeamMemberLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "Test Name",
        Description = "Test Description",
        CreatedAt = DateTimeOffset.UtcNow
    };

    public DeleteTeamMemberLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<TeamMember, TeamMemberLocalization>>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteEntity()
    {
        SetupDependencies();
        var handler = new DeleteTeamMemberLocalizationHandler(_mockLocalizationService.Object);

        var result = await handler.Handle(
            new DeleteTeamMemberLocalizationCommand(_testEntity.EntityId, _testEntity.LanguageId),
            CancellationToken.None);
        var response = new DeleteTeamMemberLocalizationDto
        {
            EntityId = _testEntity.EntityId,
            LanguageId = _testEntity.LanguageId
        };

        Assert.True(result.IsSuccess);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new DeleteTeamMemberLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteTeamMemberLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";

        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new DeleteTeamMemberLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteTeamMemberLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(notFoundMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new DeleteTeamMemberLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteTeamMemberLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync((_testEntity.EntityId, _testEntity.LanguageId));
    }
}
