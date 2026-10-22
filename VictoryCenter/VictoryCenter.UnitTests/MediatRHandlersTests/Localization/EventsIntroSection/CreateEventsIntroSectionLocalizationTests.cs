using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.EventsIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using EventsIntroSectionEntity = VictoryCenter.DAL.Entities.EventsIntroSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.EventsIntroSection;

public class CreateEventsIntroSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateEventsIntroSectionLocalizationCommand> _validator;
    private readonly CreateEventsIntroSectionLocalizationHandler _handler;

    private readonly CreateEventsIntroSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        EventsBlockTitle = "What has been happening",
        PageDescription = "This section is about the moments when what we believe in becomes real.",
    };

    private readonly EventsIntroSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        EventsBlockTitle = "What has been happening",
        PageDescription = "This section is about the moments when what we believe in becomes real.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly EventsIntroSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        EventsBlockTitle = "What has been happening",
        PageDescription = "This section is about the moments when what we believe in becomes real.",
    };

    public CreateEventsIntroSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<EventsIntroSectionEntity, EventsIntroSectionLocalization>>();
        _validator = new CreateEventsIntroSectionLocalizationValidator(
            new BaseEventsIntroSectionLocalizationValidator());
        _handler = new CreateEventsIntroSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.EventsBlockTitle, result.Value.EventsBlockTitle);
        Assert.Equal(_responseDto.PageDescription, result.Value.PageDescription);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldCreatePartialLocalization_WhenOnlyOneFieldIsProvided()
    {
        var partialDto = new CreateEventsIntroSectionLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            PageDescription = "Valid partial description.",
        };
        var partialEntity = new EventsIntroSectionLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            PageDescription = partialDto.PageDescription,
        };
        _mockMapper.Setup(mapper => mapper.Map<EventsIntroSectionLocalization>(partialDto)).Returns(partialEntity);
        _mockMapper.Setup(mapper => mapper.Map<EventsIntroSectionLocalizationDto>(partialEntity)).Returns(new EventsIntroSectionLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
            PageDescription = partialDto.PageDescription,
            TranslationStatus = VictoryCenter.DAL.Enums.TranslationStatus.Relevant,
        });
        _mockLocalizationService.Setup(service => service.CreateEntityLocalizationAsync(partialEntity))
            .ReturnsAsync(partialEntity);

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(partialDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.EventsBlockTitle);
        Assert.Equal(partialDto.PageDescription, result.Value.PageDescription);
        _mockLocalizationService.Verify(service => service.CreateEntityLocalizationAsync(partialEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateEventsIntroSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            EventsBlockTitle = "",
            PageDescription = "",
        };

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("EventsBlockTitle is required", result.Errors.Select(e => e.Message));
        Assert.Contains("PageDescription is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(EventsIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateEventsIntroSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(EventsIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<EventsIntroSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
