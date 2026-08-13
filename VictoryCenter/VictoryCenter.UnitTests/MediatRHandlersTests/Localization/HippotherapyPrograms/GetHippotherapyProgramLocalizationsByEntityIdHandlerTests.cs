using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyPrograms;

public class GetHippotherapyProgramLocalizationsByEntityIdHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<HippotherapyProgramLocalization> _localizations;
    private readonly IEnumerable<HippotherapyProgramLocalizationDto> _dtos;

    public GetHippotherapyProgramLocalizationsByEntityIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _localizations = new List<HippotherapyProgramLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Program 1",
                Description = "Description 1",
                CreatedAt = DateTimeOffset.UtcNow,
                Language = new LocalizationLanguage { Id = 1, Name = "English", Code = "en" },
                Entity = new HippotherapyProgram
                {
                    Id = 1,
                    Sections = new List<HippotherapyProgramSection>
                    {
                        new()
                        {
                            Id = 1,
                            Contents = new List<ProgramSectionContent>
                            {
                                new TitleProgramContent
                                {
                                    Id = 1,
                                    Localizations = new List<ProgramSectionContentLocalization>
                                    {
                                        new()
                                        {
                                            EntityId = 1,
                                            LanguageId = 1,
                                            CreatedAt = DateTimeOffset.UtcNow,
                                            Title = "Content 1",
                                            Description = "Content Description 1",
                                            Language = new LocalizationLanguage { Id = 1, Name = "English", Code = "en" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        _dtos = new List<HippotherapyProgramLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                Name = "Program 1",
                Description = "Description 1",
                Sections = new List<HippotherapyProgramSectionLocalizationDto>()
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenEntityIdExists()
    {
        SetupRepositoryWrapper(_localizations);
        SetupMapper(_dtos);

        var handler = new GetHippotherapyProgramLocalizationByEntityIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object);

        var result = await handler.Handle(new GetHippotherapyProgramLocalizationByEntityIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Equal("Program 1", result.Value.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldIncludeEmptyOutdatedPlaceholder_ForContentWithoutLocalization()
    {
        var translatedContent = new TitleProgramContent
        {
            Id = 1,
            Localizations =
            [
                new()
                {
                    EntityId = 1,
                    LanguageId = 1,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Title = "Content 1",
                    Language = new LocalizationLanguage { Id = 1, Name = "English", Code = "en" }
                }

            ]
        };
        var newlyAddedContent = new DescriptionProgramContent
        {
            Id = 2,
            Localizations = []
        };

        var localizations = new List<HippotherapyProgramLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Program 1",
                Description = "Description 1",
                CreatedAt = DateTimeOffset.UtcNow,
                Language = new LocalizationLanguage { Id = 1, Name = "English", Code = "en" },
                Entity = new HippotherapyProgram
                {
                    Id = 1,
                    Sections =
                    [
                        new()
                        {
                            Id = 1,
                            Contents = [translatedContent, newlyAddedContent]
                        }

                    ]
                }
            }
        };

        SetupRepositoryWrapper(localizations);
        SetupMapper(_dtos);

        var handler = new GetHippotherapyProgramLocalizationByEntityIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object);

        var result = await handler.Handle(new GetHippotherapyProgramLocalizationByEntityIdQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var contents = Assert.Single(result.Value.First().Sections).Contents;
        Assert.Equal(2, contents.Count);

        var placeholder = Assert.Single(contents, c => c.EntityId == 2);
        Assert.Equal(TranslationStatus.Outdated, placeholder.TranslationStatus);
        Assert.Null(placeholder.Title);
        Assert.Null(placeholder.Description);
        Assert.Equal(1, placeholder.LocalizationInfoDto.Id);
        Assert.Equal("en", placeholder.LocalizationInfoDto.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        SetupRepositoryWrapper(new List<HippotherapyProgramLocalization>());
        SetupMapper(new List<HippotherapyProgramLocalizationDto>());

        var handler = new GetHippotherapyProgramLocalizationByEntityIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object);

        var result = await handler.Handle(new GetHippotherapyProgramLocalizationByEntityIdQuery(999), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<HippotherapyProgramLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper.Setup(r => r.HippotherapyProgramsLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<HippotherapyProgramLocalizationDto> dtosToReturn)
    {
        _mockMapper.Setup(mapper =>
                mapper.Map<IEnumerable<HippotherapyProgramLocalizationDto>>(It.IsAny<IEnumerable<HippotherapyProgramLocalization>>()))
            .Returns(dtosToReturn);

        _mockMapper.Setup(mapper =>
                mapper.Map<HippotherapyProgramSectionContentLocalizationDto>(It.IsAny<ProgramSectionContentLocalization>()))
            .Returns(new HippotherapyProgramSectionContentLocalizationDto
            {
                EntityId = 1,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                Title = "Content 1",
                Description = "Content Description 1"
            });

        _mockMapper.Setup(mapper =>
                mapper.Map<LocalizationInfoDto>(It.IsAny<LocalizationLanguage>()))
            .Returns((LocalizationLanguage src) => new LocalizationInfoDto { Id = src.Id, Code = src.Code });
    }
}
