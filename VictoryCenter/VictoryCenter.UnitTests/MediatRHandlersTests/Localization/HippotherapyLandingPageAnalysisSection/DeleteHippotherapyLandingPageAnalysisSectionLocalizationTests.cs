using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnalysisSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAnalysisSection;

public class DeleteHippotherapyLandingPageAnalysisSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization>> _mockLocalizationService;
    private readonly DeleteHippotherapyLandingPageAnalysisSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageAnalysisSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization>>();
        _handler = new DeleteHippotherapyLandingPageAnalysisSectionLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageAnalysisSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageAnalysisSectionLocalization)),
            result.Errors[0].Message);
    }
}
