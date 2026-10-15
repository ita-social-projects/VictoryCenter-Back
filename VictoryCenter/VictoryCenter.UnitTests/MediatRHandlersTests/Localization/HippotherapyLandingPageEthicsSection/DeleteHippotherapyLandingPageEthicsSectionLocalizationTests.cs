using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageEthicsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageEthicsSection;

public class DeleteHippotherapyLandingPageEthicsSectionLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization>> _mockLocalizationService;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeleteHippotherapyLandingPageEthicsSectionLocalizationTests()
    {
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization>>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler(
            _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        SetupPrinciples([]);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalization_Successfully()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LanguageId);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageEthicsPrincipleLocalization, bool>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeletePrincipleLocalizationsForLanguage_WhenSectionHasPrinciples()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));
        SetupPrinciples(
        [
            new HippotherapyLandingPageEthicsPrinciple { Id = 10, EthicsSectionId = _entityId },
            new HippotherapyLandingPageEthicsPrinciple { Id = 11, EthicsSectionId = _entityId },
        ]);

        Expression<Func<HippotherapyLandingPageEthicsPrincipleLocalization, bool>>? capturedFilter = null;
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository.BulkDeleteAsync(
                It.IsAny<Expression<Func<HippotherapyLandingPageEthicsPrincipleLocalization, bool>>>()))
            .Callback<Expression<Func<HippotherapyLandingPageEthicsPrincipleLocalization, bool>>>(f => capturedFilter = f)
            .ReturnsAsync(2);

        var command = new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedFilter);
        var filter = capturedFilter!.Compile();
        Assert.True(filter(new HippotherapyLandingPageEthicsPrincipleLocalization { EntityId = 10, LanguageId = _languageId }));
        Assert.False(filter(new HippotherapyLandingPageEthicsPrincipleLocalization { EntityId = 10, LanguageId = _languageId + 1 }));
        Assert.False(filter(new HippotherapyLandingPageEthicsPrincipleLocalization { EntityId = 99, LanguageId = _languageId }));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(_entityId, _languageId);
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

        var command = new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageEthicsSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService
            .Setup(x => x.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageEthicsSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupPrinciples(List<HippotherapyLandingPageEthicsPrinciple> principles)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsPrinciplesRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsPrinciple>>()))
            .ReturnsAsync(principles);
    }
}
