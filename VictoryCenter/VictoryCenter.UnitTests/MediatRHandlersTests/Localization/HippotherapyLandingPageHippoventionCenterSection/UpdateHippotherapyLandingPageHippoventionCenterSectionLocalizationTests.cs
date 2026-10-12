using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionCenterSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto _updateDto = new()
    {
        Title = "Updated title",
        Description = "Updated description",
        Pros = "Includes regular sessions with qualified specialists.",
    };

    private readonly HippotherapyLandingPageHippoventionCenterSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Updated title",
        Description = "Updated description",
        Pros = "Includes regular sessions with qualified specialists.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageHippoventionCenterSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Updated title",
        Description = "Updated description",
        Pros = "Includes regular sessions with qualified specialists.",
    };

    public UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization>>();
        _validator = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator(
            new BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator());
        _handler = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.Description, result.Value.Description);
        Assert.Equal(_responseDto.Pros, result.Value.Pros);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto { Title = "", Description = "", Pros = "" };
        var command = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Pros is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
