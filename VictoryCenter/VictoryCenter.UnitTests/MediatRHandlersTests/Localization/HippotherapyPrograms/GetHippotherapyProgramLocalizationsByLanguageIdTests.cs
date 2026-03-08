using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyPrograms;

public class GetHippotherapyProgramLocalizationsByLanguageIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<HippotherapyProgramLocalization> _localizationsEntities;
    private readonly IEnumerable<HippotherapyProgramLocalizationDto> _localizationsDtos;

    public GetHippotherapyProgramLocalizationsByLanguageIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

        var language = new LocalizationLanguage
        {
            Id = 1,
            Code = "en",
            Name = "English",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _localizationsEntities = new List<HippotherapyProgramLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 1,
                Language = language,
                Name = "Program 1",
                Description = "Description 1",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                EntityId = 2,
                LanguageId = 1,
                Language = language,
                Name = "Program 2",
                Description = "Description 2",
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        _localizationsDtos = new List<HippotherapyProgramLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                Name = "Program 1",
                Description = "Description 1"
            },
            new()
            {
                EntityId = 2,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                Name = "Program 2",
                Description = "Description 2"
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenLanguageIdExists()
    {
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetHippotherapyProgramLocalizationByLanguageIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object);

        var result = await handler.Handle(new GetHippotherapyProgramLocalizationByLanguageIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal("Program 1", first.Name),
            second => Assert.Equal("Program 2", second.Name));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        SetupRepositoryWrapper(new List<HippotherapyProgramLocalization>());
        SetupMapper(new List<HippotherapyProgramLocalizationDto>());
        var handler = new GetHippotherapyProgramLocalizationByLanguageIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object);

        var result = await handler.Handle(new GetHippotherapyProgramLocalizationByLanguageIdQuery(99), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<HippotherapyProgramLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper.Setup(repo =>
                repo.HippotherapyProgramsLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<HippotherapyProgramLocalizationDto> dtosToReturn)
    {
        _mockMapper.Setup(mapper =>
                mapper.Map<List<HippotherapyProgramLocalizationDto>>(It.IsAny<IEnumerable<HippotherapyProgramLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
