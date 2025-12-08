using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class GetTeamMembersTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    public GetTeamMembersTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 4)]
    [InlineData(0, 3)]
    [InlineData(1, 2)]
    public async Task Handle_ShouldReturnSuccessfully_NoFilters(int pageNumber, int pageSize)
    {
        // Arrange
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .OrderBy(t => t.Priority)
            .Skip(pageNumber)
            .Take(pageSize)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = pageNumber,
            Limit = pageSize,
            Status = null,
            CategoryId = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        var teamMemberDtoListOld = GetTeamMemberDtoList();
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEqual(teamMemberDtoListOld.Count, result.Value.Items.Length),
            () => Assert.NotEqual(teamMemberDtoListOld, result.Value.Items),
            () => Assert.Equal(teamMemberDtoList.Count, result.Value.Items.Length),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByStatus()
    {
        // Arrange
        var status = Status.Draft;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Status == status)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            CategoryId = null
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByCategoryId()
    {
        // Arrange
        var category = new TeamCategory { Id = 2, Name = "Category 2" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.CategoryId == category.Id)
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = null,
            CategoryId = category.Id
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterByStatusAndCategoryId()
    {
        // Arrange
        var status = Status.Published;
        var category = new TeamCategory { Id = 1, Name = "Category 1" };
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Status == status && t.CategoryId == category.Id)
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            CategoryId = category.Id
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Theory]
    [InlineData(TranslationStatusFilter.All)]
    [InlineData(null)]
    public async Task Handle_ShouldReturnSuccessfully_FilterTranslationStatusFilterAllOrNull(TranslationStatusFilter? translationStatusFilter)
    {
        // Arrange
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            TranslationStatusFilter = translationStatusFilter,
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterTranslationStatusFilterMissing()
    {
        // Arrange
        int languagesCount = 2;
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Localizations.Count() < languagesCount)
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            TranslationStatusFilter = TranslationStatusFilter.Missing,
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfully_FilterTranslationStatusFilterOutdated()
    {
        // Arrange
        var teamMemberList = GetTeamMemberList();
        var teamMemberDtoList = GetTeamMemberDtoList()
            .Where(t => t.Localizations.Any(l => l.TranslationStatus == TranslationStatus.Outdated))
            .OrderBy(t => t.Priority)
            .ToList();

        SetupRepository(teamMemberList);
        SetupMapper(teamMemberDtoList);

        var filtersDto = new TeamMembersFilterDto
        {
            Offset = 0,
            Limit = 0,
            TranslationStatusFilter = TranslationStatusFilter.Outdated,
        };

        var handler = new GetTeamMembersByFiltersHandler(_mockMapper.Object, _mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetTeamMembersByFiltersQuery(filtersDto), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.NotNull(result.Value),
            () => Assert.NotEmpty(result.Value.Items),
            () => Assert.Equal(teamMemberDtoList, result.Value.Items),
            () => Assert.Equal(teamMemberList.Count, result.Value.TotalItemsCount));
    }

    private static List<TeamMember> GetTeamMemberList()
    {
        var teamMemberList = new List<TeamMember>
        {
            new()
            {
                Id = 4,
                Priority = 3,
                Status = Status.Draft,
                CategoryId = 1,
                TeamCategory = new TeamCategory { Id = 1, Name = "Category 1" },
                Localizations = new List<TeamMemberLocalization>
                {
                    new()
                    {
                        EntityId = 4,
                        LanguageId = 2,
                        TranslationStatus = TranslationStatus.Relevant,
                    },
                    new()
                    {
                        EntityId = 4,
                        LanguageId = 1,
                        TranslationStatus = TranslationStatus.Relevant,
                    }
                }
            },
            new()
            {
                Id = 2,
                Priority = 2,
                Status = Status.Draft,
                CategoryId = 2,
                TeamCategory = new TeamCategory { Id = 2, Name = "Category 2" },
                Localizations = new List<TeamMemberLocalization>
                {
                    new()
                    {
                        EntityId = 2,
                        LanguageId = 2,
                        TranslationStatus = TranslationStatus.Relevant,
                    }
                }
            },
            new()
            {
                Id = 3,
                Priority = 1,
                Status = Status.Published,
                CategoryId = 1,
                TeamCategory = new TeamCategory { Id = 1, Name = "Category 1" },
                Localizations = new List<TeamMemberLocalization>
                {
                    new()
                    {
                        EntityId = 3,
                        LanguageId = 2,
                        TranslationStatus = TranslationStatus.Outdated,
                    }
                }
            },
            new()
            {
                Id = 5,
                Priority = 2,
                Status = Status.Published,
                CategoryId = 2,
                TeamCategory = new TeamCategory { Id = 2, Name = "Category 2" }
            },
            new()
            {
                Id = 1,
                Priority = 1,
                Status = Status.Draft,
                CategoryId = 1,
                TeamCategory = new TeamCategory { Id = 1, Name = "Category 1" }
            },
        };

        return teamMemberList;
    }

    private static List<TeamMemberDto> GetTeamMemberDtoList()
    {
        var teamMemberDtoList = new List<TeamMemberDto>
        {
            new()
            {
                Id = 1,
                Priority = 1,
                Status = Status.Draft,
                CategoryId = 1,
            },
            new()
            {
                Id = 3,
                Priority = 1,
                Status = Status.Published,
                CategoryId = 1,
                Localizations = new List<TeamMemberLocalizationDto>
                {
                    new()
                    {
                        EntityId = 3,
                        TranslationStatus = TranslationStatus.Outdated,
                    }
                }
            },
            new()
            {
                Id = 2,
                Priority = 2,
                Status = Status.Draft,
                CategoryId = 12,
                Localizations = new List<TeamMemberLocalizationDto>
                {
                    new()
                    {
                        EntityId = 2,
                        TranslationStatus = TranslationStatus.Relevant,
                    }
                }
            },
            new()
            {
                Id = 5,
                Priority = 2,
                Status = Status.Published,
                CategoryId = 2
            },
            new()
            {
                Id = 4,
                Priority = 3,
                Status = Status.Draft,
                CategoryId = 1,
                Localizations = new List<TeamMemberLocalizationDto>
                {
                    new()
                    {
                        EntityId = 4,
                        TranslationStatus = TranslationStatus.Relevant,
                    },
                    new()
                    {
                        EntityId = 4,
                        TranslationStatus = TranslationStatus.Relevant,
                    }
                },
            },
        };

        return teamMemberDtoList;
    }

    private void SetupRepository(IEnumerable<TeamMember> teamMembers)
    {
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository.GetAllAsync(
             It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMembers);
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository.CountAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(teamMembers.Count);
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.LocalizationLanguagesRepository.CountAsync(
             It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(2);
    }

    private void SetupMapper(List<TeamMemberDto> teamMemberDTOList)
    {
        _mockMapper
            .Setup(x => x.Map<List<TeamMemberDto>>(It.IsAny<List<TeamMember>>()))
            .Returns(teamMemberDTOList);
    }
}
