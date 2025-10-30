using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.Public.WhoWeArePage;
using VictoryCenter.BLL.Queries.Public.WhoWeAre.GetWhoWeArePage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class GetWhoWeArePageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IMapper> _mapper;

    public GetWhoWeArePageHandlerTests()
    {
        _repositoryWrapper = new Mock<IRepositoryWrapper>();
        _mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllSections()
    {
        // Arrange
        var entities = GetEntities();
        var expectedDtos = GetDtos();

        SetupRepositoryWrapper(entities);
        SetupMapper(expectedDtos);

        _mapper
            .Setup(m => m.Map<List<WhoWeArePageSectionDto>>(entities))
            .Returns(expectedDtos);

        var handler = new GetWhoWeArePageHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetWhoWeArePageQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedDtos.Count, result.Value.Count);

        for (int i = 0; i < expectedDtos.Count; i++)
        {
            Assert.Equal(expectedDtos[i].SectionType, result.Value[i].SectionType);
            Assert.Equal(expectedDtos[i].Contents.Count, result.Value[i].Contents.Count);
        }
    }

    private void SetupRepositoryWrapper(List<WhoWeAreSection> entities)
    {
        _repositoryWrapper
            .Setup(r => r.WhoWeAreSectionsRepository.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(entities);
    }

    private void SetupMapper(List<WhoWeArePageSectionDto> expectedDtos)
    {
        _mapper
            .Setup(m => m.Map<List<WhoWeArePageSectionDto>>(It.IsAny<List<WhoWeAreSection>>()))
            .Returns(expectedDtos);
    }

    private static List<WhoWeAreSection> GetEntities() => new()
    {
        new WhoWeAreSection
        {
            Id = 1,
            SectionType = SectionType.Main,
            Title = "Основне",
            Contents = new()
            {
                new ImageContent
                {
                    Id = 1,
                    ContentType = ContentType.Image,
                    Image = new Image
                    {
                        Id = 1,
                        Url = "main.jpg",
                        MimeType = "image/jpeg"
                    }
                },
                new TitleContent
                {
                    Id = 2,
                    ContentType = ContentType.Title,
                    Title = "Простір довіри"
                },
                new DescriptionContent
                {
                    Id = 3,
                    ContentType = ContentType.Description,
                    Description = "Victory Center — це не про терміни чи цифри. Це про відчуття."
                }
            }
        },
        new WhoWeAreSection
        {
            Id = 2,
            SectionType = SectionType.WhatWeDo,
            Title = "Що ми робимо",
            Contents = new()
            {
                new DescriptionContent
                {
                    Id = 4,
                    ContentType = ContentType.Description,
                    Description = "Ми створюємо терапевтичні програми."
                }
            }
        },
        new WhoWeAreSection
        {
            Id = 3,
            SectionType = SectionType.WhoWeSupport,
            Title = "Кого підтримуємо",
            Contents = new()
            {
                new CardContent
                {
                    Id = 5,
                    ContentType = ContentType.Card,
                    Description = "Ветерани та цивільні",
                    Image = new Image
                    {
                        Id = 2,
                        Url = "support.jpg",
                        MimeType = "image/jpeg"
                    }
                }
            }
        },
        new WhoWeAreSection
        {
            Id = 4,
            SectionType = SectionType.Team,
            Title = "Команда",
            Contents = new()
            {
                new ImageContent
                {
                    Id = 6,
                    ContentType = ContentType.Image,
                    Image = new Image
                    {
                        Id = 3,
                        Url = "team.jpg",
                        MimeType = "image/jpeg"
                    }
                },
                new DescriptionContent
                {
                    Id = 7,
                    ContentType = ContentType.Description,
                    Description = "Наша команда підтримки"
                }
            }
        },
        new WhoWeAreSection
        {
            Id = 5,
            SectionType = SectionType.People,
            Title = "Люди",
            Contents = new()
            {
                new CardContent
                {
                    Id = 8,
                    ContentType = ContentType.Card,
                    Description = "Партнери та волонтери"
                }
            }
        }
    };

    private static List<WhoWeArePageSectionDto> GetDtos() => new()
    {
        new WhoWeArePageSectionDto
        {
            SectionType = SectionType.Main,
            Contents = new()
            {
                new ImageContentDto
                {
                    Id = 1,
                    ContentType = ContentType.Image,
                    Image = new ImageDto
                    {
                        Id = 1,
                        Url = "main.jpg",
                        MimeType = "image/jpeg"
                    }
                },
                new TitleContentDto
                {
                    Id = 2,
                    ContentType = ContentType.Title,
                    Title = "Простір довіри"
                },
                new DescriptionContentDto
                {
                    Id = 3,
                    ContentType = ContentType.Description,
                    Description = "Victory Center — це не про терміни чи цифри. Це про відчуття."
                }
            }
        },
        new WhoWeArePageSectionDto
        {
            SectionType = SectionType.WhatWeDo,
            Contents = new()
            {
                new DescriptionContentDto
                {
                    Id = 4,
                    ContentType = ContentType.Description,
                    Description = "Ми створюємо терапевтичні програми."
                }
            }
        },
        new WhoWeArePageSectionDto
        {
            SectionType = SectionType.WhoWeSupport,
            Contents = new()
            {
                new CardContentDto
                {
                    Id = 5,
                    ContentType = ContentType.Card,
                    Description = "Ветерани та цивільні",
                    Image = new ImageDto
                    {
                        Id = 2,
                        Url = "support.jpg",
                        MimeType = "image/jpeg"
                    }
                }
            }
        },
        new WhoWeArePageSectionDto
        {
            SectionType = SectionType.Team,
            Contents = new()
            {
                new ImageContentDto
                {
                    Id = 6,
                    ContentType = ContentType.Image,
                    Image = new ImageDto
                    {
                        Id = 3,
                        Url = "team.jpg",
                        MimeType = "image/jpeg"
                    }
                },
                new DescriptionContentDto
                {
                    Id = 7,
                    ContentType = ContentType.Description,
                    Description = "Наша команда підтримки"
                }
            }
        },
        new WhoWeArePageSectionDto
        {
            SectionType = SectionType.People,
            Contents = new()
            {
                new CardContentDto
                {
                    Id = 8,
                    ContentType = ContentType.Card,
                    Description = "Партнери та волонтери"
                }
            }
        }
    };
}
