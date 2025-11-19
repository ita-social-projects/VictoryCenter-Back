using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Validators.Localization.Languages;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.Languages;

public class UpdateLocalizationLanguageTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateLocalizationLanguageCommand> _validator;

    private readonly LocalizationLanguage _testExistingLanguage = new()
    {
        Id = 1,
        Code = "en",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    };

    private readonly LocalizationLanguage _testUpdatedLanguage = new()
    {
        Id = 1,
        Code = "uk",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    };

    private readonly LocalizationLanguageDto _testUpdatedLanguageDto = new()
    {
        Code = "uk"
    };

    public UpdateLocalizationLanguageTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateLocalizationLanguageValidator(new BaseLocalizationLanguageValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalizationLanguage()
    {
        // Arrange
        SetupDependencies(_testExistingLanguage);
        string newCode = "uk";

        var handler = new UpdateLocalizationLanguageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateLocalizationLanguageCommand(
                new UpdateLocalizationLanguageDto { Code = newCode },
                _testExistingLanguage.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(newCode, result.Value.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("e")]
    [InlineData("eng")]
    [InlineData(null)]
    public async Task Handle_ShouldNotUpdate_InvalidCode(string? invalidCode)
    {
        // Arrange
        SetupDependencies(_testExistingLanguage);

        var handler = new UpdateLocalizationLanguageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateLocalizationLanguageCommand(
                new UpdateLocalizationLanguageDto { Code = invalidCode ?? string.Empty },
                _testExistingLanguage.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Code", result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_ShouldFail_NotFound(long testId)
    {
        // Arrange
        SetupDependencies(null);
        var handler = new UpdateLocalizationLanguageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateLocalizationLanguageCommand(
                new UpdateLocalizationLanguageDto { Code = "fr" },
                testId),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(LocalizationLanguage)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        // Arrange
        SetupDependencies(_testExistingLanguage, 0);
        var handler = new UpdateLocalizationLanguageHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateLocalizationLanguageCommand(
                new UpdateLocalizationLanguageDto { Code = "fr" },
                _testExistingLanguage.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(LocalizationLanguage)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupDependencies(_testExistingLanguage, 0);
        _mockRepositoryWrapper.Setup(r =>
               r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
           .ReturnsAsync(_testExistingLanguage);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new UpdateLocalizationLanguageHandler(
           _mockMapper.Object,
           _mockRepositoryWrapper.Object,
           _validator);

        // Act
        var result = await handler.Handle(
            new UpdateLocalizationLanguageCommand(
                new UpdateLocalizationLanguageDto { Code = "fr" },
                _testExistingLanguage.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(LocalizationLanguage)), result.Errors[0].Message);
    }

    private void SetupDependencies(LocalizationLanguage? languageToReturn = null, int saveResult = 1)
    {
        SetupMapper();
        SetupRepositoryWrapper(languageToReturn, saveResult);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(m => m.Map<UpdateLocalizationLanguageDto, LocalizationLanguage>(It.IsAny<UpdateLocalizationLanguageDto>()))
            .Returns(_testUpdatedLanguage);

        _mockMapper.Setup(m => m.Map<LocalizationLanguage, LocalizationLanguageDto>(It.IsAny<LocalizationLanguage>()))
            .Returns(_testUpdatedLanguageDto);
    }

    private void SetupRepositoryWrapper(LocalizationLanguage? entity = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(r =>
                r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(entity);

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
