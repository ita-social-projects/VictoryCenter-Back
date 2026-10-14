using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageScientificReferencesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageScientificReferencesSection;

public class DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization>>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
            _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        SetupReferences([]);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LanguageId);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageScientificReferenceLocalization, bool>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteReferenceLocalizationsForLanguage_WhenSectionHasReferences()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));
        SetupReferences(
        [
            new HippotherapyLandingPageScientificReference { Id = 10, ScientificReferencesSectionId = _entityId },
            new HippotherapyLandingPageScientificReference { Id = 11, ScientificReferencesSectionId = _entityId },
        ]);

        Expression<Func<HippotherapyLandingPageScientificReferenceLocalization, bool>>? capturedFilter = null;
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageScientificReferenceLocalization, bool>>>()))
            .Callback<Expression<Func<HippotherapyLandingPageScientificReferenceLocalization, bool>>>(f => capturedFilter = f)
            .ReturnsAsync(2);

        var command = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedFilter);
        var filter = capturedFilter!.Compile();
        Assert.True(filter(new HippotherapyLandingPageScientificReferenceLocalization { EntityId = 10, LanguageId = _languageId }));
        Assert.False(filter(new HippotherapyLandingPageScientificReferenceLocalization { EntityId = 10, LanguageId = _languageId + 1 }));
        Assert.False(filter(new HippotherapyLandingPageScientificReferenceLocalization { EntityId = 99, LanguageId = _languageId }));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupReferences(List<HippotherapyLandingPageScientificReference> references)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferencesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReference>>()))
            .ReturnsAsync(references);
    }
}
