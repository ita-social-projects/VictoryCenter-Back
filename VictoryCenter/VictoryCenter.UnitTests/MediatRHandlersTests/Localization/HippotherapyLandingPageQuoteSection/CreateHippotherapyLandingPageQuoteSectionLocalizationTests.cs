using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageQuoteSection;

public class CreateHippotherapyLandingPageQuoteSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageQuoteSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageQuoteSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageQuoteSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
    };

    private readonly HippotherapyLandingPageQuoteSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageQuoteSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
    };

    public CreateHippotherapyLandingPageQuoteSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageQuoteSectionLocalizationValidator(
            new BaseHippotherapyLandingPageQuoteSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageQuoteSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_responseDto.QuoteText, result.Value.QuoteText);
        Assert.Equal(_responseDto.AuthorName, result.Value.AuthorName);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            QuoteText = "",
            AuthorName = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("QuoteText is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
