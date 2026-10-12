using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnotherQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto _updateDto = new()
    {
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
    };

    private readonly HippotherapyLandingPageAnotherQuoteSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageAnotherQuoteSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
    };

    public UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization>>();
        _validator = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator(
            new BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator());
        _handler = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.QuoteText, result.Value.QuoteText);
        Assert.Equal(_responseDto.AuthorName, result.Value.AuthorName);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto { QuoteText = "", AuthorName = "" };
        var command = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("QuoteText is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
