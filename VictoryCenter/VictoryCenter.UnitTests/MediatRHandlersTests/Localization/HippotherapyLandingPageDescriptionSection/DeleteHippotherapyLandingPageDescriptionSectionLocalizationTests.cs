using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageDescriptionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageDescriptionSection;

public class DeleteHippotherapyLandingPageDescriptionSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization>> _mockLocalizationService;
    private readonly DeleteHippotherapyLandingPageDescriptionSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageDescriptionSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization>>();
        _handler = new DeleteHippotherapyLandingPageDescriptionSectionLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageDescriptionSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageDescriptionSectionLocalization)),
            result.Errors[0].Message);
    }
}
