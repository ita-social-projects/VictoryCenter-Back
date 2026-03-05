using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class GetPreviewsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IMapper> _mapper;

    public GetPreviewsTests()
    {
        _repositoryWrapper = new Mock<IRepositoryWrapper>();
        _mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSectionPreviews()
    {
        // Arrange
        var entities = GetEntities();
        var expectedDtos = GetDtos();

        SetupRepositoryWrapper(entities);
        SetupMapper(expectedDtos);

        var handler = new GetWhoWeAreSectionPreviewsHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetWhoWeAreSectionPreviewsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedDtos.Count, result.Value.Count);

        for (int i = 0; i < expectedDtos.Count; i++)
        {
            Assert.Equal(expectedDtos[i].Id, result.Value[i].Id);
            Assert.Equal(expectedDtos[i].Title, result.Value[i].Title);
            Assert.Equal(expectedDtos[i].SectionType, result.Value[i].SectionType);
        }
    }

    [Fact]
    public async Task Handle_WithTranslationStatuses_ShouldAggregateCorrectly()
    {
        // Arrange
        var entities = GetEntitiesWithTranslationStatuses();
        var expectedDtos = GetDtosWithTranslationStatuses();

        SetupRepositoryWrapper(entities);
        SetupMapper(expectedDtos);

        var handler = new GetWhoWeAreSectionPreviewsHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetWhoWeAreSectionPreviewsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var sectionDto = result.Value[0];
        Assert.Equal(expectedDtos[0].Id, sectionDto.Id);
        Assert.Equal(expectedDtos[0].Title, sectionDto.Title);
        Assert.Equal(expectedDtos[0].SectionType, sectionDto.SectionType);

        var statuses = sectionDto.TranslationStatuses;
        Assert.NotNull(statuses);
        Assert.Equal(2, statuses.Count);
        Assert.Contains(statuses, s => s.LanguageId == 1 && s.TranslationStatus == TranslationStatus.Relevant);
        Assert.Contains(statuses, s => s.LanguageId == 2 && s.TranslationStatus == TranslationStatus.Outdated);
        Assert.DoesNotContain(statuses, s => s.LanguageId == 3);
        Assert.DoesNotContain(statuses, s => s.LanguageId == 4);
    }

    [Fact]
    public async Task Handle_WithImageOnlyContents_ShouldReturnEmptyTranslationStatuses()
    {
        // Arrange
        var entities = GetEntitiesWithImageOnlyContents();
        var expectedDtos = GetDtosWithTranslationStatuses();

        SetupRepositoryWrapper(entities);
        SetupMapper(expectedDtos);

        var handler = new GetWhoWeAreSectionPreviewsHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetWhoWeAreSectionPreviewsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var statuses = result.Value[0].TranslationStatuses;
        Assert.NotNull(statuses);
        Assert.Empty(statuses);
    }

    private void SetupMapper(List<WhoWeAreSectionInfoDto> dtos)
    {
        _mapper
            .Setup(m => m.Map<WhoWeAreSectionInfoDto>(It.IsAny<WhoWeAreSection>()))
            .Returns((WhoWeAreSection source) => dtos.FirstOrDefault(d => d.Id == source.Id)!);
    }

    private void SetupRepositoryWrapper(List<WhoWeAreSection> entities)
    {
        _repositoryWrapper
            .Setup(r => r.WhoWeAreSectionsRepository.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(entities);
    }

    private static List<WhoWeAreSection> GetEntities() => new()
    {
        new WhoWeAreSection
        {
            Id = 1,
            SectionType = SectionType.Main,
            Title = "Основне",
            CreatedAt = DateTime.Now,
            Contents = [],
        },
        new WhoWeAreSection
        {
            Id = 2,
            SectionType = SectionType.WhatWeDo,
            Title = "Що ми робимо",
            CreatedAt = DateTime.Now,
            Contents = [],
        },
        new WhoWeAreSection
        {
            Id = 3,
            SectionType = SectionType.WhoWeSupport,
            Title = "Кого ми підтримуємо",
            CreatedAt = DateTime.Now,
            Contents = []
        },
        new WhoWeAreSection
        {
            Id = 4,
            SectionType = SectionType.Team,
            Title = "Команда",
            CreatedAt = DateTime.Now,
            Contents = []
        },
        new WhoWeAreSection
        {
            Id = 5,
            SectionType = SectionType.People,
            Title = "Люди",
            CreatedAt = DateTime.Now,
            Contents = []
        }
    };

    private static List<WhoWeAreSectionInfoDto> GetDtos() => new()
    {
        new WhoWeAreSectionInfoDto()
        {
            Id = 1,
            Title = "Основне",
            SectionType = "Main"
        },
        new WhoWeAreSectionInfoDto()
        {
            Id = 2,
            Title = "Що ми робимо",
            SectionType = "WhatWeDo"
        },
        new WhoWeAreSectionInfoDto()
        {
            Id = 3,
            Title = "Кого ми підтримуємо",
            SectionType = "WhoWeSupport"
        },
        new WhoWeAreSectionInfoDto()
        {
            Id = 4,
            Title = "Команда",
            SectionType = "Team"
        },
        new WhoWeAreSectionInfoDto()
        {
            Id = 5,
            Title = "Люди",
            SectionType = "People"
        }
    };

    private static List<WhoWeAreSection> GetEntitiesWithTranslationStatuses() => new()
    {
        new WhoWeAreSection
        {
            Id = 1,
            SectionType = SectionType.Main,
            Title = "Test Support",
            CreatedAt = DateTime.Now,
            Contents = new List<WhoWeAreContent>
            {
                new TitleContent
                {
                    Id = 1,
                    ContentType = ContentType.Title,
                    Localizations = new List<WhoWeAreContentLocalization>
                    {
                        new() { LanguageId = 1, TranslationStatus = TranslationStatus.Relevant },
                        new() { LanguageId = 2, TranslationStatus = TranslationStatus.Relevant },
                        new() { LanguageId = 3, TranslationStatus = TranslationStatus.Outdated }
                    }
                },
                new DescriptionContent
                {
                    Id = 2,
                    ContentType = ContentType.Description,
                    Localizations = new List<WhoWeAreContentLocalization>
                    {
                        new() { LanguageId = 1, TranslationStatus = TranslationStatus.Relevant },
                        new() { LanguageId = 2, TranslationStatus = TranslationStatus.Outdated }
                    }
                },
                new ImageContent
                {
                    Id = 3,
                    ContentType = ContentType.Image,
                    Localizations = new List<WhoWeAreContentLocalization>
                    {
                        new() { LanguageId = 4, TranslationStatus = TranslationStatus.Relevant }
                    }
                }
            }
        }
    };

    private static List<WhoWeAreSection> GetEntitiesWithImageOnlyContents() => new()
    {
        new WhoWeAreSection
        {
            Id = 1,
            SectionType = SectionType.Main,
            Title = "Test Support",
            CreatedAt = DateTime.Now,
            Contents = new List<WhoWeAreContent>
            {
                new ImageContent
                {
                    Id = 3,
                    ContentType = ContentType.Image,
                    Localizations = new List<WhoWeAreContentLocalization>
                    {
                        new() { LanguageId = 1, TranslationStatus = TranslationStatus.Outdated },
                        new() { LanguageId = 2, TranslationStatus = TranslationStatus.Relevant }
                    }
                }
            }
        }
    };

    private static List<WhoWeAreSectionInfoDto> GetDtosWithTranslationStatuses() => new()
    {
        new WhoWeAreSectionInfoDto
        {
            Id = 1,
            Title = "Test Support",
            SectionType = "Main"
        }
    };
}
