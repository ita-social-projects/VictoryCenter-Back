using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using PartnersPageBannerEntity = VictoryCenter.DAL.Entities.PartnersPageBanner;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnersPageBanner;

public class DeletePartnersPageBannerLocalizationHandlerTests
{
    private readonly Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>> _mockLocalizationService;
    private readonly DeletePartnersPageBannerLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    public DeletePartnersPageBannerLocalizationHandlerTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>>();
        _handler = new DeletePartnersPageBannerLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeletePartnersPageBannerLocalization_Successfully()
    {
        _mockLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ReturnsAsync((_entityId, _languageId));

        var command = new DeletePartnersPageBannerLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LanguageId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new KeyNotFoundException("Localization not found."));

        var command = new DeletePartnersPageBannerLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Localization not found.", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new InvalidOperationException());

        var command = new DeletePartnersPageBannerLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockLocalizationService.Setup(s => s.DeleteEntityLocalizationAsync(_entityId, _languageId))
            .ThrowsAsync(new DbUpdateException());

        var command = new DeletePartnersPageBannerLocalizationCommand(_entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }
}
