using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Common.Localization.Languages.GetAll;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.Languages;

public class GetLocalizationLanguagesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<LocalizationLanguage> _testLocalizationLanguages = new List<LocalizationLanguage>
    {
        new()
        {
            Id = 1,
            Code = "en"
        },
        new()
        {
            Id = 2,
            Code = "es"
        },
    };

    private readonly IEnumerable<LocalizationLanguageDto> _testLocalizationLanguageDtos = new List<LocalizationLanguageDto>
    {
        new()
        {
            Id = 1,
            Code = "en"
        },
        new()
        {
            Id = 2,
            Code = "es"
        },
    };

    public GetLocalizationLanguagesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllLocalizationLanguages()
    {
        // Arrange
        SetupDependencies();
        var handler = new GetAllLocalizationLanguagesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetAllLocalizationLanguagesQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoLanguagesExist()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(repo => repo.LocalizationLanguagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new List<LocalizationLanguage>());

        _mockMapper.Setup(x => x.Map<IEnumerable<LocalizationLanguageDto>>(It.IsAny<IEnumerable<LocalizationLanguage>>()))
            .Returns(new List<LocalizationLanguageDto>());

        var handler = new GetAllLocalizationLanguagesHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetAllLocalizationLanguagesQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupDependencies()
    {
        _mockRepositoryWrapper.Setup(repo => repo.LocalizationLanguagesRepository.GetAllAsync(
                It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(_testLocalizationLanguages);

        _mockMapper.Setup(x => x.Map<IEnumerable<LocalizationLanguageDto>>(It.IsAny<IEnumerable<LocalizationLanguage>>()))
            .Returns(_testLocalizationLanguageDtos);
    }
}
