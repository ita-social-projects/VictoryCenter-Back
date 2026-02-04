using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamCategories;
public class DeleteTeamCategoriesLocalizationTests
{
    private readonly Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>> _mockLocalizationService;
    private readonly DeleteTeamCategoryLocalizationHandler _handler;
    private readonly TeamCategoryLocalization _localizedTeamCategoryLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Upd Localized Name",
        Description = "Upd Localized Description",
    };
    public DeleteTeamCategoriesLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>>();
        _handler = new DeleteTeamCategoryLocalizationHandler(_mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteTeamCategoryLocalization_Successfully()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync((_localizedTeamCategoryLocalization.EntityId, _localizedTeamCategoryLocalization.LanguageId));
        var command = new DeleteTeamCategoryLocalizationCommand(_localizedTeamCategoryLocalization.EntityId, _localizedTeamCategoryLocalization.LanguageId);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException("Localization not found."));
        var command = new DeleteTeamCategoryLocalizationCommand(_localizedTeamCategoryLocalization.EntityId, _localizedTeamCategoryLocalization.LanguageId);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Localization not found.", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new DbUpdateException());
        var command = new DeleteTeamCategoryLocalizationCommand(_localizedTeamCategoryLocalization.EntityId, _localizedTeamCategoryLocalization.LanguageId);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        _mockLocalizationService.Setup(x => x.DeleteEntityLocalizationAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new InvalidOperationException());
        var command = new DeleteTeamCategoryLocalizationCommand(_localizedTeamCategoryLocalization.EntityId, _localizedTeamCategoryLocalization.LanguageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }
}
