using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnalysisSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAnalysisSection;

public class CreateHippotherapyLandingPageAnalysisSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageAnalysisSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageAnalysisSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Analysis",
        Description = "Therapeutic horseback riding for children and veterans.",
    };

    private readonly HippotherapyLandingPageAnalysisSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Analysis",
        Description = "Therapeutic horseback riding for children and veterans.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageAnalysisSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Analysis",
        Description = "Therapeutic horseback riding for children and veterans.",
    };

    public CreateHippotherapyLandingPageAnalysisSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageAnalysisSectionLocalizationValidator(
            new BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageAnalysisSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.Description, result.Value.Description);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageAnalysisSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Description = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnalysisSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnalysisSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAnalysisSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnalysisSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAnalysisSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnalysisSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnalysisSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
