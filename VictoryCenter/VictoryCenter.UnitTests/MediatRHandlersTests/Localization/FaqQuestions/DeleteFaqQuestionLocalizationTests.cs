using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FaqQuestions;

public class DeleteFaqQuestionLocalizationTests
{
    private readonly Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>> _mockLocalizationService;

    private readonly FaqQuestionLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        QuestionText = "Super mega question text that has enough symbols",
        AnswerText = "Ultra detailed and long enought answer text, that will definetely pass validation",
        CreatedAt = DateTime.UtcNow
    };

    public DeleteFaqQuestionLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteEntity()
    {
        SetupDependencies();
        var handler = new DeleteFaqQuestionLocalizationHandler(_mockLocalizationService.Object);

        var result = await handler.Handle(
            new DeleteFaqQuestionLocalizationCommand(_testEntity.EntityId, _testEntity.LanguageId),
            CancellationToken.None);
        var response = new DeleteFaqQuestionLocalizationDto
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

        var handler = new DeleteFaqQuestionLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteFaqQuestionLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";

        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new DeleteFaqQuestionLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteFaqQuestionLocalizationCommand(1, 1);

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

        var handler = new DeleteFaqQuestionLocalizationHandler(_mockLocalizationService.Object);

        var command = new DeleteFaqQuestionLocalizationCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync((_testEntity.EntityId, _testEntity.LanguageId));
    }
}
