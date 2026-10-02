using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageIntroSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageIntroSection;

public class UpdateHippotherapyLandingPageIntroSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageIntroSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageIntroSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateHippotherapyLandingPageIntroSectionLocalizationDto _updateDto = new()
    {
        Title = "Updated title",
        Description = "Updated description",
    };

    private readonly HippotherapyLandingPageIntroSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Updated title",
        Description = "Updated description",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageIntroSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Updated title",
        Description = "Updated description",
    };

    public UpdateHippotherapyLandingPageIntroSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization>>();
        _validator = new UpdateHippotherapyLandingPageIntroSectionLocalizationValidator(
            new BaseHippotherapyLandingPageIntroSectionLocalizationValidator());
        _handler = new UpdateHippotherapyLandingPageIntroSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.Description, result.Value.Description);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto { Title = "", Description = "" };
        var command = new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageIntroSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageIntroSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageIntroSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
