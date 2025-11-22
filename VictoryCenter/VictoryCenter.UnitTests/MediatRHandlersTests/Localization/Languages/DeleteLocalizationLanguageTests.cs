using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.Languages;

public class DeleteLocalizationLanguageTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly LocalizationLanguage _testExistingLanguage = new()
    {
        Id = 1,
        Code = "en",
        Name = "Англійська",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    };

    public DeleteLocalizationLanguageTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteLocalizationLanguage()
    {
        // Arrange
        SetupRepositoryWrapper(_testExistingLanguage);
        var handler = new DeleteLocalizationLanguageHandler(_mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new DeleteLocalizationLanguageCommand(_testExistingLanguage.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_testExistingLanguage.Id, result.Value);
        _mockRepositoryWrapper.Verify(r => r.LocalizationLanguagesRepository.Delete(_testExistingLanguage), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldFail_WhenLanguageNotFound(long id)
    {
        // Arrange
        SetupRepositoryWrapper(null);
        var handler = new DeleteLocalizationLanguageHandler(_mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new DeleteLocalizationLanguageCommand(id), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(id, typeof(LocalizationLanguage)), result.Errors[0].Message);
        _mockRepositoryWrapper.Verify(r => r.LocalizationLanguagesRepository.Delete(It.IsAny<LocalizationLanguage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupRepositoryWrapper(_testExistingLanguage, 0);
        var handler = new DeleteLocalizationLanguageHandler(_mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new DeleteLocalizationLanguageCommand(_testExistingLanguage.Id), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(LocalizationLanguage)), result.Errors[0].Message);
        _mockRepositoryWrapper.Verify(r => r.LocalizationLanguagesRepository.Delete(_testExistingLanguage), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(r =>
               r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
           .ReturnsAsync(_testExistingLanguage);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new DeleteLocalizationLanguageHandler(_mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new DeleteLocalizationLanguageCommand(_testExistingLanguage.Id), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(LocalizationLanguage)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(LocalizationLanguage? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(r =>
                r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
