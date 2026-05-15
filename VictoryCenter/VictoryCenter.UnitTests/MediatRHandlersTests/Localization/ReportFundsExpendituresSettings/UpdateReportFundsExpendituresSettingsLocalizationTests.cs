using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities.Localization;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateReportFundsExpendituresSettingsLocalizationCommand> _validator;
    private readonly UpdateReportFundsExpendituresSettingsLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateReportFundsExpendituresSettingsLocalizationDto _updateDto = new()
    {
        DisclaimerTitle = "Updated disclaimer text"
    };

    private readonly ReportFundsExpendituresSettingsLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        DisclaimerTitle = "Updated disclaimer text",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" }
    };

    private readonly ReportFundsExpendituresSettingsLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        DisclaimerTitle = "Updated disclaimer text"
    };

    public UpdateReportFundsExpendituresSettingsLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization>>();
        _validator = new UpdateReportFundsExpendituresSettingsLocalizationValidator(
            new BaseReportFundsExpendituresSettingsLocalizationValidator());
        _handler = new UpdateReportFundsExpendituresSettingsLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateReportFundsExpendituresSettingsLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateReportFundsExpendituresSettingsLocalizationDto { DisclaimerTitle = "" };
        var command = new UpdateReportFundsExpendituresSettingsLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("DisclaimerTitle is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateReportFundsExpendituresSettingsLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateReportFundsExpendituresSettingsLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresSettingsLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateReportFundsExpendituresSettingsLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresSettingsLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
