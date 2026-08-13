using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyProgramCategories.GetByEntityId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyProgramCategories;

public class GetHippotherapyProgramCategoryLocalizationByEntityIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetHippotherapyProgramCategoryLocalizationByEntityIdHandler _handler;

    private readonly List<HippotherapyProgramCategoryLocalization> _entities =
    [
        new HippotherapyProgramCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "English Name",
            Language = new LocalizationLanguage { Id = 2, Code = "en", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        },
        new HippotherapyProgramCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 3,
            Name = "German Name",
            Language = new LocalizationLanguage { Id = 3, Code = "de", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        }

    ];

    private readonly List<HippotherapyProgramCategoryLocalizationDto> _dtos =
    [
        new HippotherapyProgramCategoryLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
            Name = "English Name"
        },
        new HippotherapyProgramCategoryLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 3, Code = "de" },
            Name = "German Name"
        }

    ];

    public GetHippotherapyProgramCategoryLocalizationByEntityIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetHippotherapyProgramCategoryLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenEntityIdExists()
    {
        SetupRepositoryWrapper(_entities);
        SetupMapper(_dtos);

        var query = new GetHippotherapyProgramCategoryLocalizationByEntityIdQuery(1);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenEntityIdDoesNotExist()
    {
        SetupRepositoryWrapper([]);
        SetupMapper([]);

        var query = new GetHippotherapyProgramCategoryLocalizationByEntityIdQuery(999);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<HippotherapyProgramCategoryLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper
            .Setup(repo => repo.HippotherapyProgramCategoryLocalizationsRepository
                .GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategoryLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<HippotherapyProgramCategoryLocalizationDto> dtosToReturn)
    {
        _mockMapper
            .Setup(m => m.Map<List<HippotherapyProgramCategoryLocalizationDto>>(
                It.IsAny<IEnumerable<HippotherapyProgramCategoryLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
