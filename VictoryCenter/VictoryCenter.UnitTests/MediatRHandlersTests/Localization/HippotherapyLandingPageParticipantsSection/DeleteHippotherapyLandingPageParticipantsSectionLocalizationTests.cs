using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageParticipantsSection;

public class DeleteHippotherapyLandingPageParticipantsSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageParticipantsSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler(
            _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        SetupCards([]);
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
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageParticipantCardLocalization, bool>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCardLocalizationsForLanguage_WhenSectionHasCards()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));
        SetupCards(
        [
            new HippotherapyLandingPageParticipantCard { Id = 10, ParticipantsSectionId = _entityId },
            new HippotherapyLandingPageParticipantCard { Id = 11, ParticipantsSectionId = _entityId },
        ]);

        Expression<Func<HippotherapyLandingPageParticipantCardLocalization, bool>>? capturedFilter = null;
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageParticipantCardLocalization, bool>>>()))
            .Callback<Expression<Func<HippotherapyLandingPageParticipantCardLocalization, bool>>>(f => capturedFilter = f)
            .ReturnsAsync(2);

        var command = new DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedFilter);
        var filter = capturedFilter!.Compile();
        Assert.True(filter(new HippotherapyLandingPageParticipantCardLocalization { EntityId = 10, LanguageId = _languageId }));
        Assert.False(filter(new HippotherapyLandingPageParticipantCardLocalization { EntityId = 10, LanguageId = _languageId + 1 }));
        Assert.False(filter(new HippotherapyLandingPageParticipantCardLocalization { EntityId = 99, LanguageId = _languageId }));
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

    private void SetupCards(List<HippotherapyLandingPageParticipantCard> cards)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCard>>()))
            .ReturnsAsync(cards);
    }
}
