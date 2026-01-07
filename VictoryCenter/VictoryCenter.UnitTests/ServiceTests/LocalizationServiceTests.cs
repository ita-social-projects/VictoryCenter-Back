using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class LocalizationServiceTests
{
    private readonly ILocalizationService<TeamMember, TeamMemberLocalization> _localizationService;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();

    private readonly TeamMember _teamMember = new()
    {
        Id = 1,
        FullName = "TestName1 TestSuname1",
        Priority = 1,
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long test1 description",
        Email = "Test1@gmail.com",
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
    };

    public LocalizationServiceTests()
    {
        _localizationService = new LocalizationService<TeamMember, TeamMemberLocalization>(_repositoryWrapper.Object);
    }

    [Fact]
    public async Task CreateEntityLocalizationAsync_ShouldReturnCreatedLocalization_WhenEntityAndLanguageExistAndSaveSucceeds()
    {
        // Arrange
        var entityLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "Localized Name",
            Description = "Localized Description"
        };

        var language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" };

        var createdLocalizationWithLanguage = new TeamMemberLocalization
        {
            EntityId = entityLocalization.EntityId,
            LanguageId = entityLocalization.LanguageId,
            FullName = entityLocalization.FullName,
            Description = entityLocalization.Description,
            Language = language,
            CreatedAt = DateTimeOffset.UtcNow
        };

        SetupRepositoryWrapper(teamMember: _teamMember, localizationLanguage: language, teamMemberLocalization: createdLocalizationWithLanguage);

        _repositoryWrapper.Setup(x => x.GetRepository<TeamMemberLocalization>()
                .CreateAsync(It.IsAny<TeamMemberLocalization>()))
            .ReturnsAsync((TeamMemberLocalization lm) => lm);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _localizationService.CreateEntityLocalizationAsync(entityLocalization);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityLocalization.EntityId, result.EntityId);
        Assert.Equal(entityLocalization.LanguageId, result.LanguageId);
        Assert.Equal(entityLocalization.FullName, result.FullName);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotNull(result.Language);
        Assert.Equal(language.Id, result.Language.Id);
        Assert.Equal(language.Code, result.Language.Code);
        _repositoryWrapper.Verify(x => x.GetRepository<TeamMemberLocalization>().CreateAsync(It.IsAny<TeamMemberLocalization>()), Times.Once);
    }

    [Fact]
    public async Task CreateEntityLocalizationAsync_ShouldThrowKeyNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var entityLocalization = new TeamMemberLocalization
        {
            EntityId = 999,
            LanguageId = 1,
            FullName = "Localized Name",
            Description = "Localized Description"
        };
        SetupRepositoryWrapper();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _localizationService.CreateEntityLocalizationAsync(entityLocalization));
        Assert.Equal(ErrorMessagesConstants.NotFound(entityLocalization.EntityId, typeof(TeamMember)), ex.Message);
    }

    [Fact]
    public async Task CreateEntityLocalizationAsync_ShouldThrowKeyNotFoundException_WhenLanguageDoesNotExist()
    {
        // Arrange
        var entityLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 999,
            FullName = "Localized Name",
            Description = "Localized Description"
        };

        SetupRepositoryWrapper(teamMember: _teamMember);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _localizationService.CreateEntityLocalizationAsync(entityLocalization));
        Assert.Equal(ErrorMessagesConstants.NotFound(entityLocalization.LanguageId, typeof(LocalizationLanguage)), ex.Message);
    }

    [Fact]
    public async Task CreateEntityLocalizationAsync_ShouldThrowInvalidOperationException_WhenSaveChangesReturnsZero()
    {
        // Arrange
        var entityLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "Localized Name",
            Description = "Localized Description"
        };

        var language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" };

        SetupRepositoryWrapper(teamMember: _teamMember, localizationLanguage: language);

        _repositoryWrapper.Setup(x => x.GetRepository<TeamMemberLocalization>()
                .CreateAsync(It.IsAny<TeamMemberLocalization>()))
            .ReturnsAsync((TeamMemberLocalization lm) => lm);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _localizationService.CreateEntityLocalizationAsync(entityLocalization));
    }

    [Fact]
    public async Task UpdateEntityLocalizationAsync_ShouldReturnUpdatedLocalization_WhenLocalizationExistsAndSaveSucceeds()
    {
        // Arrange
        var existingLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "Old Name",
            Description = "Old Description",
            Language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" },
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var updatedLocalizationFromDb = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "New Localized Name",
            Description = "New Localized Description",
            TranslationStatus = TranslationStatus.Relevant,
            Language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" }
        };

        var newLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "New Localized Name",
            Description = "New Localized Description"
        };

        _repositoryWrapper
            .SetupSequence(x => x.GetRepository<TeamMemberLocalization>()
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMemberLocalization>>()))
            .ReturnsAsync(existingLocalization)
            .ReturnsAsync(updatedLocalizationFromDb);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _localizationService.UpdateEntityLocalizationAsync(newLocalization);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TranslationStatus.Relevant, result.TranslationStatus);
        Assert.Equal(newLocalization.FullName, result.FullName);
        Assert.NotNull(result.Language);
        Assert.Equal("en", result.Language.Code);

        _repositoryWrapper.Verify(
            x => x.GetRepository<TeamMemberLocalization>()
                .Update(It.Is<TeamMemberLocalization>(l => l == newLocalization)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateEntityLocalizationAsync_ShouldThrowKeyNotFoundException_WhenLocalizationDoesNotExist()
    {
        // Arrange
        var localizationToUpdate = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            FullName = "Name",
            Description = "Desc"
        };

        SetupRepositoryWrapper(teamMemberLocalization: null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _localizationService.UpdateEntityLocalizationAsync(localizationToUpdate));
        Assert.Equal(ErrorMessagesConstants.NotFound((localizationToUpdate.EntityId, localizationToUpdate.LanguageId), typeof(TeamMemberLocalization)), ex.Message);
    }

    [Fact]
    public async Task UpdateEntityLocalizationAsync_ShouldThrowInvalidOperationException_WhenSaveChangesReturnsZero()
    {
        // Arrange
        var existingLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "Old Name",
            Description = "Old Description",
            Language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" },
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var newLocalization = new TeamMemberLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "New Localized Name",
            Description = "New Localized Description"
        };

        SetupRepositoryWrapper(teamMemberLocalization: existingLocalization);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _localizationService.UpdateEntityLocalizationAsync(newLocalization));
    }

    [Fact]
    public async Task DeleteEntityLocalizationAsync_ShouldReturnTuple_WhenLocalizationExistsAndSaveSucceeds()
    {
        // Arrange
        const long entityId = 1;
        const long languageId = 1;

        var existingLocalization = new TeamMemberLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            FullName = "To delete",
            Description = "To delete"
        };

        SetupRepositoryWrapper(teamMemberLocalization: existingLocalization);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _localizationService.DeleteEntityLocalizationAsync(entityId, languageId);

        // Assert
        Assert.Equal((entityId, languageId), result);
        _repositoryWrapper.Verify(x => x.GetRepository<TeamMemberLocalization>().Delete(It.Is<TeamMemberLocalization>(l => l == existingLocalization)), Times.Once);
    }

    [Fact]
    public async Task DeleteEntityLocalizationAsync_ShouldThrowKeyNotFoundException_WhenLocalizationDoesNotExist()
    {
        // Arrange
        const long entityId = 1;
        const long languageId = 2;

        SetupRepositoryWrapper(teamMemberLocalization: null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _localizationService.DeleteEntityLocalizationAsync(entityId, languageId));
        Assert.Equal(ErrorMessagesConstants.NotFound((entityId, languageId), typeof(TeamMemberLocalization)), ex.Message);
    }

    [Fact]
    public async Task DeleteEntityLocalizationAsync_ShouldThrowInvalidOperationException_WhenSaveChangesReturnsZero()
    {
        // Arrange
        const long entityId = 1;
        const long languageId = 1;

        var existingLocalization = new TeamMemberLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            FullName = "To delete",
            Description = "To delete"
        };

        SetupRepositoryWrapper(teamMemberLocalization: existingLocalization);

        _repositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _localizationService.DeleteEntityLocalizationAsync(entityId, languageId));
    }

    private void SetupRepositoryWrapper(
        TeamMemberLocalization? teamMemberLocalization = null,
        TeamMember? teamMember = null,
        LocalizationLanguage? localizationLanguage = null)
    {
        _repositoryWrapper.Setup(x => x.GetRepository<TeamMemberLocalization>()
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMemberLocalization>>()))
            .ReturnsAsync(teamMemberLocalization);

        _repositoryWrapper.Setup(x => x.GetRepository<TeamMember>()
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMember);

        _repositoryWrapper.Setup(x => x.LocalizationLanguagesRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(localizationLanguage);
    }
}
