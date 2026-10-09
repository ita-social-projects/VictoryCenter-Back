using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.EventNews;
using VictoryCenter.DAL.Entities.Localization;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.EventNews;

public class UpdateEventNewsLocalizationTests
{
    private readonly Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<UpdateEventNewsLocalizationCommand> _validator;

    private readonly UpdateEventNewsLocalizationDto _testUpdateDto = new()
    {
        Title = "Updated Event Title",
        Description = "Updated Event Description",
        AdditionalDescription = "Updated info"
    };

    private readonly EventNewsLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Updated Event Title",
        Description = "Updated Event Description",
        AdditionalDescription = "Updated info"
    };

    private readonly EventNewsLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        Title = "Updated Event Title",
        Description = "Updated Event Description",
        AdditionalDescription = "Updated info"
    };

    public UpdateEventNewsLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<EventNewsEntity, EventNewsLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _validator = new UpdateEventNewsLocalizationValidator(new BaseEventNewsLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateEventNewsLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = new UpdateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateEventNewsLocalizationCommand(_testUpdateDto, 1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Title, result.Value.Title);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<EventNewsLocalization>(It.IsAny<UpdateEventNewsLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<UpdateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateEventNewsLocalizationCommand(_testUpdateDto, 1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";

        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<UpdateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new UpdateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateEventNewsLocalizationCommand(_testUpdateDto, 1, 1);

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
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<UpdateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new UpdateEventNewsLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateEventNewsLocalizationCommand(_testUpdateDto, 1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(x => x.Map<EventNewsLocalization>(It.IsAny<UpdateEventNewsLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<EventNewsLocalizationDto>(It.IsAny<EventNewsLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<EventNewsLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
