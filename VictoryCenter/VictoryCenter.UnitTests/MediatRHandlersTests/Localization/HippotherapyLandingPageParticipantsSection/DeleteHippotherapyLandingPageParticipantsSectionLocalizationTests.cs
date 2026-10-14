using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageParticipantsSection;

public class DeleteHippotherapyLandingPageParticipantsSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>> _mockLocalizationService;
    private readonly DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageParticipantsSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>>();
        _handler = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }
}
