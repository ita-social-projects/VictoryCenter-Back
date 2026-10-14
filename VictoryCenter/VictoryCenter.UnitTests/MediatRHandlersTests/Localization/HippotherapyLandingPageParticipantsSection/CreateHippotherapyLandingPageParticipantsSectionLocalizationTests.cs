using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageParticipantsSection;

public class CreateHippotherapyLandingPageParticipantsSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageParticipantsSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageParticipantsSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Who the programs are for",
    };

    private readonly HippotherapyLandingPageParticipantsSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Who the programs are for",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageParticipantsSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Who the programs are for",
    };

    public CreateHippotherapyLandingPageParticipantsSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageParticipantsSectionLocalizationValidator(
            new BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageParticipantsSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
