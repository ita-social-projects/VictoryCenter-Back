using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.EventsIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using EventsIntroSectionEntity = VictoryCenter.DAL.Entities.EventsIntroSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.EventsIntroSection;

public class UpdateEventsIntroSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateEventsIntroSectionLocalizationCommand> _validator;
    private readonly UpdateEventsIntroSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateEventsIntroSectionLocalizationDto _updateDto = new()
    {
        EventsBlockTitle = "Updated title",
        PageDescription = "Updated description text goes here.",
    };

    private readonly EventsIntroSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        EventsBlockTitle = "Updated title",
        PageDescription = "Updated description text goes here.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly EventsIntroSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        EventsBlockTitle = "Updated title",
        PageDescription = "Updated description text goes here.",
    };

    public UpdateEventsIntroSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization>>();
        _validator = new UpdateEventsIntroSectionLocalizationValidator(
            new BaseEventsIntroSectionLocalizationValidator());
        _handler = new UpdateEventsIntroSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateEventsIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.EventsBlockTitle, result.Value.EventsBlockTitle);
        Assert.Equal(_responseDto.PageDescription, result.Value.PageDescription);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateEventsIntroSectionLocalizationDto { EventsBlockTitle = "", PageDescription = "" };
        var command = new UpdateEventsIntroSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EventsBlockTitle is required", result.Errors.Select(e => e.Message));
        Assert.Contains("PageDescription is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateEventsIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateEventsIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventsIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateEventsIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EventsIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
