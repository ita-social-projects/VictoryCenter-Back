using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.EventNews;

public class DeleteEventNewsLocalizationTests
{
    private readonly Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>> _mockLocalizationService;

    private readonly EventNewsLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Test Title",
        Description = "Test Description",
        CreatedAt = DateTimeOffset.UtcNow
    };

    public DeleteEventNewsLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteEntity()
    {
        SetupDependencies();
        var handler = new DeleteEventNewsLocalizationHandler(_mockLocalizationService.Object);

        var result = await handler.Handle(
            new DeleteEventNewsLocalizationCommand(_testEntity.EntityId, _testEntity.LanguageId),
            CancellationToken.None);
        var response = new DeleteEventNewsLocalizationDto
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

        var handler = new DeleteEventNewsLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteEventNewsLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";

        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new DeleteEventNewsLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteEventNewsLocalizationCommand(1, 1);

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

        var handler = new DeleteEventNewsLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteEventNewsLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync((_testEntity.EntityId, _testEntity.LanguageId));
    }
}
