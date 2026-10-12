using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionCenterSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "HippoventionCenter",
        Description = "Therapeutic horseback riding for children and veterans.",
        Pros = "Includes regular sessions with qualified specialists.",
    };

    private readonly HippotherapyLandingPageHippoventionCenterSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "HippoventionCenter",
        Description = "Therapeutic horseback riding for children and veterans.",
        Pros = "Includes regular sessions with qualified specialists.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageHippoventionCenterSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "HippoventionCenter",
        Description = "Therapeutic horseback riding for children and veterans.",
        Pros = "Includes regular sessions with qualified specialists.",
    };

    public CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator(
            new BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.Description, result.Value.Description);
        Assert.Equal(_responseDto.Pros, result.Value.Pros);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Description = "",
            Pros = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Pros is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
