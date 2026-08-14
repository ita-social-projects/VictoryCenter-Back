using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnerSections;

public class DeletePartnerSectionLocalizationHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<PartnerSection, PartnerSectionLocalization>> _mockSectionLocalizationService;
    private readonly DeletePartnerSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeletePartnerSectionLocalizationHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockSectionLocalizationService = new Mock<ILocalizationService<PartnerSection, PartnerSectionLocalization>>();
        _handler = new DeletePartnerSectionLocalizationHandler(_mockRepositoryWrapper.Object, _mockSectionLocalizationService.Object);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    [Fact]
    public async Task Handle_ShouldDeleteSectionAndPartnerLocalizations_WhenPartnersExist()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerRepository.GetAllAsync(It.IsAny<QueryOptions<Partner>>()))
            .ReturnsAsync([new Partner { Id = 10, PartnersSectionId = _entityId }, new Partner { Id = 11, PartnersSectionId = _entityId }]);

        Expression<Func<PartnerLocalization, bool>>? capturedPredicate = null;
        _mockRepositoryWrapper.Setup(r => r.PartnerLocalizationsRepository.BulkDeleteAsync(It.IsAny<Expression<Func<PartnerLocalization, bool>>>()))
            .Callback<Expression<Func<PartnerLocalization, bool>>>(predicate => capturedPredicate = predicate)
            .ReturnsAsync(2);
        _mockSectionLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeletePartnerSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        _mockRepositoryWrapper.Verify(r => r.PartnerLocalizationsRepository.BulkDeleteAsync(It.IsAny<Expression<Func<PartnerLocalization, bool>>>()), Times.Once);

        Assert.NotNull(capturedPredicate);
        var predicateFunc = capturedPredicate!.Compile();

        Assert.True(predicateFunc(new PartnerLocalization { EntityId = 10, LanguageId = _languageId }));
        Assert.True(predicateFunc(new PartnerLocalization { EntityId = 11, LanguageId = _languageId }));

        Assert.False(predicateFunc(new PartnerLocalization { EntityId = 10, LanguageId = _languageId + 1 }));

        Assert.False(predicateFunc(new PartnerLocalization { EntityId = 999, LanguageId = _languageId }));
    }

    [Fact]
    public async Task Handle_ShouldSkipBulkDelete_WhenSectionHasNoPartners()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerRepository.GetAllAsync(It.IsAny<QueryOptions<Partner>>()))
            .ReturnsAsync([]);
        _mockSectionLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeletePartnerSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(r => r.PartnerLocalizationsRepository.BulkDeleteAsync(It.IsAny<Expression<Func<PartnerLocalization, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockSectionLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found."));

        var command = new DeletePartnerSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Localization not found.", result.Errors.Select(e => e.Message));

        _mockRepositoryWrapper.Verify(r => r.PartnerRepository.GetAllAsync(It.IsAny<QueryOptions<Partner>>()), Times.Never);
        _mockRepositoryWrapper.Verify(r => r.PartnerLocalizationsRepository.BulkDeleteAsync(It.IsAny<Expression<Func<PartnerLocalization, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerRepository.GetAllAsync(It.IsAny<QueryOptions<Partner>>()))
            .ReturnsAsync([]);
        _mockSectionLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new InvalidOperationException());

        var command = new DeletePartnerSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSectionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerRepository.GetAllAsync(It.IsAny<QueryOptions<Partner>>()))
            .ReturnsAsync([]);
        _mockSectionLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeletePartnerSectionLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PartnerSectionLocalization)), result.Errors[0].Message);
    }
}
