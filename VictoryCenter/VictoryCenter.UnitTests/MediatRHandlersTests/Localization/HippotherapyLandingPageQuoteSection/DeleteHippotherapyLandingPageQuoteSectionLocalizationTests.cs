using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageQuoteSection;

public class DeleteHippotherapyLandingPageQuoteSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>> _mockLocalizationService;
    private readonly DeleteHippotherapyLandingPageQuoteSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageQuoteSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization>>();
        _handler = new DeleteHippotherapyLandingPageQuoteSectionLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LanguageId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new InvalidOperationException());

        var command = new DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageQuoteSectionLocalization)),
            result.Errors[0].Message);
    }
}
