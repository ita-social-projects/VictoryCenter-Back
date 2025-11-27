using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Validators.Localization.Languages;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.Languages;

public class CreateLocalizationLanguageTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateLocalizationLanguageCommand> _validator;

    private readonly LocalizationLanguage _testEntity = new()
    {
        Id = 1,
        Code = "en",
        Name = "Англійська",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset)
    };

    private LocalizationLanguageDto _testDto = new()
    {
        Code = "en",
        Name = "Англійська"
    };

    public CreateLocalizationLanguageTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateLocalizationLanguageValidator(new BaseLocalizationLanguageValidator());
    }

    [Theory]
    [InlineData("en")]
    [InlineData("uk")]
    public async Task Handle_ShouldCreateLocalizationLanguage(string code)
    {
        // Arrange
        _testEntity.Code = code;
        _testDto = _testDto with { Code = code };
        SetupDependencies();

        var handler = new CreateLocalizationLanguageHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new CreateLocalizationLanguageCommand(new CreateLocalizationLanguageDto { Code = code, Name = "Test" }),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(code, result.Value.Code);
    }

    [Fact]
    public async Task Handle_ShouldFail_InvalidCode()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateLocalizationLanguageHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _validator);
        string code = "e";

        // Act
        var result = await handler.Handle(
            new CreateLocalizationLanguageCommand(new CreateLocalizationLanguageDto { Code = code ?? string.Empty, Name = "Test" }),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustHaveALengthOfNCharacters(nameof(CreateLocalizationLanguageDto.Code), LocalizationLanguageConstants.CodeLength),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_NameMaxedOut()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateLocalizationLanguageHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _validator);
        string code = "en";
        var name = new string('a', LocalizationLanguageConstants.NameMaxLength + 1);

        // Act
        var result = await handler.Handle(
            new CreateLocalizationLanguageCommand(new CreateLocalizationLanguageDto { Code = code, Name = name }),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateLocalizationLanguageDto.Name), LocalizationLanguageConstants.NameMaxLength),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        // Arrange
        SetupDependencies(0);
        var handler = new CreateLocalizationLanguageHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new CreateLocalizationLanguageCommand(new CreateLocalizationLanguageDto { Code = "en", Name = "Англійська" }),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(LocalizationLanguage)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_DbUpdateException()
    {
        // Arrange
        _mapperMock.Setup(m => m.Map<LocalizationLanguage>(It.IsAny<CreateLocalizationLanguageDto>()))
            .Returns(_testEntity);
        _repositoryWrapperMock
            .Setup(r => r.LocalizationLanguagesRepository.CreateAsync(It.IsAny<LocalizationLanguage>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateLocalizationLanguageHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new CreateLocalizationLanguageCommand(new CreateLocalizationLanguageDto { Code = "en", Name = "Англійська" }),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(LocalizationLanguage)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        _mapperMock.Setup(m => m.Map<LocalizationLanguage>(It.IsAny<CreateLocalizationLanguageDto>()))
            .Returns(_testEntity);
        _mapperMock.Setup(m => m.Map<LocalizationLanguageDto>(It.IsAny<LocalizationLanguage>()))
            .Returns(_testDto);

        _repositoryWrapperMock.Setup(r => r.LocalizationLanguagesRepository.CreateAsync(It.IsAny<LocalizationLanguage>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
