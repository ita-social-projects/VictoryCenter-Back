using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.WhoWeAreContents;

public class GetWhoWeAreContentLocalizationsByLanguageIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IWhoWeAreContentLocalizationsRepository> _mockLocalizationsRepository;

    private readonly IEnumerable<WhoWeAreContentLocalization> _localizationsEntities;
    private readonly IEnumerable<WhoWeAreContentLocalizationDto> _localizationsDtos;

    public GetWhoWeAreContentLocalizationsByLanguageIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationsRepository = new Mock<IWhoWeAreContentLocalizationsRepository>();

        _mockRepositoryWrapper
            .Setup(r => r.WhoWeAreContentLocalizationsRepository)
            .Returns(_mockLocalizationsRepository.Object);

        var language = new LocalizationLanguage { Id = 2, Code = "de", CreatedAt = DateTimeOffset.UtcNow };

        _localizationsEntities = new List<WhoWeAreContentLocalization>
        {
            new() { EntityId = 1, LanguageId = 2, Language = language, Title = "Hallo" },
            new() { EntityId = 2, LanguageId = 2, Language = language, Description = "Beschreibung" }
        };

        _localizationsDtos = new List<WhoWeAreContentLocalizationDto>
        {
            new() { EntityId = 1, LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "de" }, Title = "Hallo" },
            new() { EntityId = 2, LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "de" }, Description = "Beschreibung" }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenLanguageIdExists()
    {
        // Arrange
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetWhoWeAreContentLocalizationByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetWhoWeAreContentLocalizationByLanguageIdQuery(2), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal("Hallo", first.Title),
            second => Assert.Equal("Beschreibung", second.Description));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        // Arrange
        SetupRepositoryWrapper(new List<WhoWeAreContentLocalization>());
        SetupMapper(new List<WhoWeAreContentLocalizationDto>());
        var handler = new GetWhoWeAreContentLocalizationByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetWhoWeAreContentLocalizationByLanguageIdQuery(99), CancellationToken.None);

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
