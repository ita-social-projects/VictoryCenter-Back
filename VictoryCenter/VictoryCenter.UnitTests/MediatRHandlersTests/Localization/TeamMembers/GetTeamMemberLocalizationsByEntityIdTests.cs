using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByEntityId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class GetTeamMemberLocalizationsByEntityIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<TeamMemberLocalization> _localizationsEntities;
    private readonly IEnumerable<TeamMemberLocalizationDto> _localizationsDtos;

    public GetTeamMemberLocalizationsByEntityIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

        var languageEn = new LocalizationLanguage
        {
            Id = 1,
            Code = "en",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var languageDe = new LocalizationLanguage
        {
            Id = 2,
            Code = "de",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _localizationsEntities = new List<TeamMemberLocalization>
        {
            new()
            {
                EntityId = 10,
                LanguageId = 1,
                Language = languageEn,
                FullName = "John Doe",
                Description = "Team leader",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                EntityId = 10,
                LanguageId = 2,
                Language = languageDe,
                FullName = "Johann Doe",
                Description = "Teamleiter",
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        _localizationsDtos = new List<TeamMemberLocalizationDto>
        {
            new()
            {
                EntityId = 10,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                FullName = "John Doe",
                Description = "Team leader"
            },
            new()
            {
                EntityId = 10,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "de" },
                FullName = "Johann Doe",
                Description = "Teamleiter"
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenTeamMemberIdExists()
    {
        // Arrange
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetTeamMemberLocalizationByEntityIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetTeamMemberLocalizationByEntityIdQuery(10), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal("John Doe", first.FullName),
            second => Assert.Equal("Johann Doe", second.FullName));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        // Arrange
        SetupRepositoryWrapper(new List<TeamMemberLocalization>());
        SetupMapper(new List<TeamMemberLocalizationDto>());
        var handler = new GetTeamMemberLocalizationByEntityIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetTeamMemberLocalizationByEntityIdQuery(999), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<TeamMemberLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper.Setup(repo =>
            repo.TeamMemberLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<TeamMemberLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<TeamMemberLocalizationDto> dtosToReturn)
    {
        _mockMapper.Setup(mapper =>
            mapper.Map<List<TeamMemberLocalizationDto>>(It.IsAny<IEnumerable<TeamMemberLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
