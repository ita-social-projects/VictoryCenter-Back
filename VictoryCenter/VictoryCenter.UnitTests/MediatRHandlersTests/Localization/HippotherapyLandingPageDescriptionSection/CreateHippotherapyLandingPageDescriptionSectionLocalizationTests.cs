using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageDescriptionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageDescriptionSection;

public class CreateHippotherapyLandingPageDescriptionSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageDescriptionSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageDescriptionSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Hippotherapy",
        Description = "Therapeutic horseback riding for children and veterans.",
    };

    private readonly HippotherapyLandingPageDescriptionSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Hippotherapy",
        Description = "Therapeutic horseback riding for children and veterans.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageDescriptionSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Hippotherapy",
        Description = "Therapeutic horseback riding for children and veterans.",
    };

    public CreateHippotherapyLandingPageDescriptionSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageDescriptionSectionLocalizationValidator(
            new BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageDescriptionSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(_createDto),
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
        var invalidDto = new CreateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Description = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(invalidDto),
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
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageDescriptionSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageDescriptionSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageDescriptionSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageDescriptionSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageDescriptionSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageDescriptionSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageDescriptionSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
