using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.EventNews;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.EventNews;

public class CreateEventNewsLocalizationTests
{
    private readonly Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateEventNewsLocalizationCommand> _validator;

    private readonly CreateEventNewsLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Event Title",
        Description = "Event Description",
        AdditionalDescription = "Additional info"
    };

    private readonly EventNewsLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Event Title",
        Description = "Event Description",
        AdditionalDescription = "Additional info"
    };

    private readonly EventNewsLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        Title = "Event Title",
        Description = "Event Description",
        AdditionalDescription = "Additional info"
    };

    public CreateEventNewsLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _validator = new CreateEventNewsLocalizationValidator(new BaseEventNewsLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateEventNewsLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateEventNewsLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Title, result.Value.Title);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<EventNewsLocalization>(It.IsAny<CreateEventNewsLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<CreateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateEventNewsLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";

        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<CreateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new CreateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateEventNewsLocalizationCommand(_testCreateDto);

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
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<CreateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new CreateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateEventNewsLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<CreateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
