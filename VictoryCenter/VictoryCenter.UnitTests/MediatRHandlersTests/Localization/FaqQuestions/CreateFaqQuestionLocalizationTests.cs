using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FaqQuestions;

public class CreateFaqQuestionLocalizationTests
{
    private readonly Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateFaqQuestionLocalizationCommand> _validator;

    private readonly CreateFaqQuestionLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        QuestionText = "Super mega question text that has enough symbols",
        AnswerText = "Ultra detailed and long enought answer text, that will definetely pass validation",
    };

    private readonly FaqQuestionLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        QuestionText = "Super mega question text",
        AnswerText = "Ultra detailed and long enought answer text",
    };

    private readonly FaqQuestionLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        QuestionText = "Super mega question text",
        AnswerText = "Ultra detailed and long enought answer text",
    };

    public CreateFaqQuestionLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<FaqQuestion, FaqQuestionLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _validator = new CreateFaqQuestionLocalizationValidator(new BaseFaqQuestionLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateFaqQuestionLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateFaqQuestionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.QuestionText, result.Value.QuestionText);
        Assert.Equal(_testDto.AnswerText, result.Value.AnswerText);
        Assert.Equal(_testDto.EntityId, result.Value.EntityId);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<FaqQuestionLocalization>(It.IsAny<CreateFaqQuestionLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<FaqQuestionLocalization>(It.IsAny<CreateFaqQuestionLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<FaqQuestionLocalizationDto>(It.IsAny<FaqQuestionLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateFaqQuestionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        _mockMapper.Setup(x => x.Map<FaqQuestionLocalization>(It.IsAny<CreateFaqQuestionLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new CreateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateFaqQuestionLocalizationCommand(_testCreateDto);

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
        _mockMapper.Setup(x => x.Map<FaqQuestionLocalization>(It.IsAny<CreateFaqQuestionLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new CreateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateFaqQuestionLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(FaqQuestionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateFaqQuestionLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            QuestionText = "", // invalid
            AnswerText = "Too short"
        };

        var handler = new CreateFaqQuestionLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateFaqQuestionLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(FaqQuestionLocalization.QuestionText)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(x => x.Map<FaqQuestionLocalization>(It.IsAny<CreateFaqQuestionLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<FaqQuestionLocalizationDto>(It.IsAny<FaqQuestionLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<FaqQuestionLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
