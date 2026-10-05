using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageQuoteSection;

public class UpdateHippotherapyLandingPageQuoteSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageQuoteSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdateHippotherapyLandingPageQuoteSectionLocalizationDto _updateDto = new()
    {
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
    };

    private readonly HippotherapyLandingPageQuoteSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageQuoteSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        QuoteText = "Updated quote text",
        AuthorName = "Updated author",
    };

    public UpdateHippotherapyLandingPageQuoteSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>>();
        _validator = new UpdateHippotherapyLandingPageQuoteSectionLocalizationValidator(
            new BaseHippotherapyLandingPageQuoteSectionLocalizationValidator());
        _handler = new UpdateHippotherapyLandingPageQuoteSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.QuoteText, result.Value.QuoteText);
        Assert.Equal(_responseDto.AuthorName, result.Value.AuthorName);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto { QuoteText = "", AuthorName = "" };
        var command = new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("QuoteText is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageQuoteSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);
    }
}
