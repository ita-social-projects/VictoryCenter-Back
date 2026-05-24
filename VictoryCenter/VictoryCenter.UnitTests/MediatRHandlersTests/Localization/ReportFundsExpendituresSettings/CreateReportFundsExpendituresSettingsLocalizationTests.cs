using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities.Localization;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.ReportFundsExpendituresSettings;

public class CreateReportFundsExpendituresSettingsLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateReportFundsExpendituresSettingsLocalizationCommand> _validator;
    private readonly CreateReportFundsExpendituresSettingsLocalizationHandler _handler;

    private readonly CreateReportFundsExpendituresSettingsLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        DisclaimerTitle = "English disclaimer text"
    };

    private readonly ReportFundsExpendituresSettingsLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        DisclaimerTitle = "English disclaimer text",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" }
    };

    private readonly ReportFundsExpendituresSettingsLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        DisclaimerTitle = "English disclaimer text"
    };

    public CreateReportFundsExpendituresSettingsLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization>>();
        _validator = new CreateReportFundsExpendituresSettingsLocalizationValidator(
            new BaseReportFundsExpendituresSettingsLocalizationValidator());
        _handler = new CreateReportFundsExpendituresSettingsLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            DisclaimerTitle = ""
        };

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("DisclaimerTitle is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresSettingsLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresSettingsLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresSettingsLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
