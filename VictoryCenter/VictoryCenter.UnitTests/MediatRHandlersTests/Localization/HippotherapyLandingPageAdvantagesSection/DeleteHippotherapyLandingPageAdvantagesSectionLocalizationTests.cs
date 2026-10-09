using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class DeleteHippotherapyLandingPageAdvantagesSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageAdvantagesSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
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

        var command = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LanguageId);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageAdvantageCardLocalization, bool>>>()),
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
            new HippotherapyLandingPageAdvantageCard { Id = 10, AdvantagesSectionId = _entityId },
            new HippotherapyLandingPageAdvantageCard { Id = 11, AdvantagesSectionId = _entityId },
        ]);

        Expression<Func<HippotherapyLandingPageAdvantageCardLocalization, bool>>? capturedFilter = null;
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageAdvantageCardLocalization, bool>>>()))
            .Callback<Expression<Func<HippotherapyLandingPageAdvantageCardLocalization, bool>>>(f => capturedFilter = f)
            .ReturnsAsync(2);

        var command = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedFilter);
        var filter = capturedFilter!.Compile();
        Assert.True(filter(new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 10, LanguageId = _languageId }));
        Assert.False(filter(new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 10, LanguageId = _languageId + 1 }));
        Assert.False(filter(new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 99, LanguageId = _languageId }));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupCards(List<HippotherapyLandingPageAdvantageCard> cards)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCard>>()))
            .ReturnsAsync(cards);
    }
}
