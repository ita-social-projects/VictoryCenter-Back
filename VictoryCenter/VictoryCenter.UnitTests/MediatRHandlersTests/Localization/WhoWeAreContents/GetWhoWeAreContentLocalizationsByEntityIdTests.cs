using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByEntityId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.WhoWeAreContents;

public class GetWhoWeAreContentLocalizationsByEntityIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IWhoWeAreContentLocalizationsRepository> _mockLocalizationsRepository;

    private readonly IEnumerable<WhoWeAreContentLocalization> _localizationsEntities;
    private readonly IEnumerable<WhoWeAreContentLocalizationDto> _localizationsDtos;

    public GetWhoWeAreContentLocalizationsByEntityIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationsRepository = new Mock<IWhoWeAreContentLocalizationsRepository>();

        _mockRepositoryWrapper
            .Setup(r => r.WhoWeAreContentLocalizationsRepository)
            .Returns(_mockLocalizationsRepository.Object);

        var languageEn = new LocalizationLanguage { Id = 1, Code = "en", CreatedAt = DateTimeOffset.UtcNow };
        var languageDe = new LocalizationLanguage { Id = 2, Code = "de", CreatedAt = DateTimeOffset.UtcNow };

        _localizationsEntities = new List<WhoWeAreContentLocalization>
        {
            new() { EntityId = 10, LanguageId = 1, Language = languageEn, Title = "Hello" },
            new() { EntityId = 10, LanguageId = 2, Language = languageDe, Title = "Hallo" }
        };

        _localizationsDtos = new List<WhoWeAreContentLocalizationDto>
        {
            new() { EntityId = 10, LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" }, Title = "Hello" },
            new() { EntityId = 10, LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "de" }, Title = "Hallo" }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenEntityIdExists()
    {
        // Arrange
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetWhoWeAreContentLocalizationByEntityIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetWhoWeAreContentLocalizationByEntityIdQuery(10), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal("Hello", first.Title),
            second => Assert.Equal("Hallo", second.Title));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        // Arrange
        SetupRepositoryWrapper(new List<WhoWeAreContentLocalization>());
        SetupMapper(new List<WhoWeAreContentLocalizationDto>());
        var handler = new GetWhoWeAreContentLocalizationByEntityIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetWhoWeAreContentLocalizationByEntityIdQuery(999), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<WhoWeAreContentLocalization> entitiesToReturn)
    {
        _mockLocalizationsRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreContentLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<WhoWeAreContentLocalizationDto> dtosToReturn)
    {
        _mockMapper
            .Setup(mapper => mapper.Map<List<WhoWeAreContentLocalizationDto>>(It.IsAny<IEnumerable<WhoWeAreContentLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
