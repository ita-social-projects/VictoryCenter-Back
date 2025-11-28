using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FaqQuestions;

public class UpdateFaqQuestionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateFaqQuestionLocalizationCommand> _validator;

    private readonly FaqQuestionLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        QuestionText = "OLD super mega question text that has enough symbols",
        AnswerText = "OLD ultra detailed and long enought answer text, that will definetely pass validation",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly FaqQuestionLocalization _updatedEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        QuestionText = "NEW super mega question text that has enough symbols",
        AnswerText = "NEW ultra detailed and long enought answer text, that will definetely pass validation",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly UpdateFaqQuestionLocalizationDto _updatedDto = new()
    {
        QuestionText = "NEW super mega question text that has enough symbols",
        AnswerText = "NEW ultra detailed and long enought answer text, that will definetely pass validation",
    };

    private readonly FaqQuestionLocalizationDto _updatedTestDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        QuestionText = "NEW super mega question text that has enough symbols",
        AnswerText = "NEW ultra detailed and long enought answer text, that will definetely pass validation",
    };

    private readonly long _entityId = 1;
    private readonly long _languageId = 1;

    public UpdateFaqQuestionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>>();
        _validator = new UpdateFaqQuestionLocalizationValidator(new BaseFaqQuestionLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateFaqQuestionLocalization_Successfully()
    {
        // Arrange
        SetupDependencies(_updatedEntity);
        var handler = new UpdateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);
        long entityId = _testEntity.EntityId;
        long languageId = _testEntity.LanguageId;

        var command = new UpdateFaqQuestionLocalizationCommand(_updatedDto, entityId, languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_updatedDto.QuestionText, result.Value.QuestionText);
        Assert.Equal(_updatedDto.AnswerText, result.Value.AnswerText);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<FaqQuestionLocalization>(It.IsAny<UpdateFaqQuestionLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupMapper();

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateFaqQuestionLocalizationCommand(_updatedDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        SetupMapper();

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new UpdateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateFaqQuestionLocalizationCommand(_updatedDto, _entityId, _languageId);

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
        _mockMapper.Setup(x => x.Map<FaqQuestionLocalization>(It.IsAny<UpdateFaqQuestionLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new UpdateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateFaqQuestionLocalizationCommand(_updatedDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new UpdateFaqQuestionLocalizationDto
        {
            QuestionText = "", // invalid
            AnswerText = "Too short"
        };

        var handler = new UpdateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateFaqQuestionLocalizationCommand(invalidDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(FaqQuestionLocalization.QuestionText)), result.Errors[0].Message);
    }

    private void SetupDependencies(FaqQuestionLocalization? entityToReturn = null)
    {
        SetupMapper();
        SetupLocalizationService(entityToReturn);
    }

    private void SetupLocalizationService(FaqQuestionLocalization? entityToReturn = null)
    {
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ReturnsAsync(entityToReturn);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(m => m.Map<FaqQuestionLocalization>(It.IsAny<UpdateFaqQuestionLocalizationDto>()))
            .Returns(_updatedEntity);

        _mockMapper.Setup(m => m.Map(It.IsAny<UpdateFaqQuestionLocalizationDto>(), It.IsAny<FaqQuestionLocalization>()))
            .Returns(_updatedEntity);

        _mockMapper.Setup(m => m.Map<FaqQuestionLocalizationDto>(It.IsAny<FaqQuestionLocalization>()))
            .Returns(_updatedTestDto);
    }
}
