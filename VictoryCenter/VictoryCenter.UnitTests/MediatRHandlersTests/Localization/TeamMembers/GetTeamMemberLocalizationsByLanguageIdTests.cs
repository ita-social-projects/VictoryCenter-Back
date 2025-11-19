using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class GetTeamMemberLocalizationsByLanguageIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<TeamMemberLocalization> _localizationsEntities;
    private readonly IEnumerable<TeamMemberLocalizationDto> _localizationsDtos;

    public GetTeamMemberLocalizationsByLanguageIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

        var language = new LocalizationLanguage
        {
            Id = 1,
            Code = "en",
            CreatedAt = DateTime.UtcNow
        };

        _localizationsEntities = new List<TeamMemberLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 1,
                Language = language,
                FullName = "John Doe",
                Description = "Team lead",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                EntityId = 2,
                LanguageId = 1,
                Language = language,
                FullName = "Jane Smith",
                Description = "Developer",
                CreatedAt = DateTime.UtcNow
            }
        };

        _localizationsDtos = new List<TeamMemberLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                LocalizationLanguageDto = new LocalizationLanguageDto { Id = 1, Code = "en" },
                FullName = "John Doe",
                Description = "Team lead"
            },
            new()
            {
                EntityId = 2,
                LocalizationLanguageDto = new LocalizationLanguageDto { Id = 1, Code = "en" },
                FullName = "Jane Smith",
                Description = "Developer"
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenLanguageIdExists()
    {
        // Arrange
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetByLanguageIdQuery(1), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal("John Doe", first.FullName),
            second => Assert.Equal("Jane Smith", second.FullName));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        // Arrange
        SetupRepositoryWrapper(new List<TeamMemberLocalization>());
        SetupMapper(new List<TeamMemberLocalizationDto>());
        var handler = new GetByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetByLanguageIdQuery(99), CancellationToken.None);

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
