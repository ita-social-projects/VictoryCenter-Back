using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamCategories;
public class GetTeamCategoriesLocalizationByLanguageId
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetTeamCategoryLocalizationByLanguageIdHandler _handler;
    private readonly List<TeamCategoryLocalization> _localizationsEntities = new()
    {
        new TeamCategoryLocalization
        {
            EntityId = 5,
            LanguageId = 2,
            Language = new LocalizationLanguage
            {
                Id = 2,
                Code = "en",
                CreatedAt = DateTimeOffset.UtcNow
            },
            Name = "Category A",
            Description = "Description A",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new TeamCategoryLocalization
        {
            EntityId = 5,
            LanguageId = 2,
            Language = new LocalizationLanguage
            {
                Id = 2,
                Code = "fr",
                CreatedAt = DateTimeOffset.UtcNow
            },
            Name = "Category A",
            Description = "Description A",
            CreatedAt = DateTimeOffset.UtcNow
        }
    };
    private readonly List<TeamCategoryLocalizationDto> _localizationsDtos = new()
    {
        new TeamCategoryLocalizationDto
        {
            EntityId = 5,
            LocalizationInfoDto = new LocalizationInfoDto
            {
                Id = 2,
                Code = "en"
            },
            Name = "Category A",
            Description = "Description A",
        },
        new TeamCategoryLocalizationDto
        {
            EntityId = 5,
            LocalizationInfoDto = new LocalizationInfoDto
            {
                Id = 2,
                Code = "en"
            },
            Name = "Category A",
            Description = "Description A",
        }
    };

    public GetTeamCategoriesLocalizationByLanguageId()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetTeamCategoryLocalizationByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizedTeamCategories_WhenLanguageIdExists()
    {
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);

        var query = new GetTeamCategoryLocalizationByLanguageIdQuery(2);

        var result = await _handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmplyList_WhenLanguageIdNotExist()
    {
        SetupRepositoryWrapper(new List<TeamCategoryLocalization>());
        SetupMapper(new List<TeamCategoryLocalizationDto>());

        var query = new GetTeamCategoryLocalizationByLanguageIdQuery(10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<TeamCategoryLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper.Setup(repo =>
            repo.TeamCategoryLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<TeamCategoryLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<TeamCategoryLocalizationDto> dtosToReturn)
    {
        _mockMapper.Setup(mapper =>
            mapper.Map<List<TeamCategoryLocalizationDto>>(It.IsAny<IEnumerable<TeamCategoryLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
