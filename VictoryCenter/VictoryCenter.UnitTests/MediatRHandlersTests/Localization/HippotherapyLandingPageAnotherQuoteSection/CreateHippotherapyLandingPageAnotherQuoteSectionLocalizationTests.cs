using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnotherQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler _handler;

    private readonly CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
    };

    private readonly HippotherapyLandingPageAnotherQuoteSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageAnotherQuoteSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        QuoteText = "Hippotherapy changed my child's life.",
        AuthorName = "Parent of a patient",
    };

    public CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization>>();
        _validator = new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator(
            new BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator());
        _handler = new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_createDto),
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
        var invalidDto = new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            QuoteText = "",
            AuthorName = "",
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("QuoteText is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
