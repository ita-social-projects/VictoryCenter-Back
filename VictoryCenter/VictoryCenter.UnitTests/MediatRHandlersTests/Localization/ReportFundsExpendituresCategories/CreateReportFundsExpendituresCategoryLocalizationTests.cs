using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateReportFundsExpendituresCategoryLocalizationCommand> _validator;
    private readonly CreateReportFundsExpendituresCategoryLocalizationHandler _handler;

    private readonly CreateReportFundsExpendituresCategoryLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "English Name"
    };

    private readonly ReportFundsExpendituresCategoryLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "English Name",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" }
    };

    private readonly ReportFundsExpendituresCategoryLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Name = "English Name"
    };

    public CreateReportFundsExpendituresCategoryLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization>>();
        _validator = new CreateReportFundsExpendituresCategoryLocalizationValidator(
            new BaseReportFundsExpendituresCategoryLocalizationValidator());
        _handler = new CreateReportFundsExpendituresCategoryLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.Name, result.Value.Name);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Name = ""
        };

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Name is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresCategoryLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresCategoryLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresCategoryLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresCategoryLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresCategoryLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresCategoryLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<ReportFundsExpendituresCategoryLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
