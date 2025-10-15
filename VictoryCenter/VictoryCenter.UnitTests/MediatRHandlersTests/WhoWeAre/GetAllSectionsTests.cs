using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class GetAllSectionsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IMapper> _mapper;

    public GetAllSectionsTests()
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

        var handler = new GetAllWhoWeAreSectionsHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetAllWhoWeAreSectionsQuery();

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

    private void SetupMapper(List<WhoWeAreSectionInfoDto> dtos)
    {
        _mapper
            .Setup(m => m.Map<List<WhoWeAreSectionInfoDto>>(It.IsAny<List<WhoWeAreSection>>()))
            .Returns(dtos);
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
            Contents = null!,
        },
        new WhoWeAreSection
        {
            Id = 2,
            SectionType = SectionType.WhatWeDo,
            Title = "Що ми робимо",
            CreatedAt = DateTime.Now,
            Contents = null!,
        },
        new WhoWeAreSection
        {
            Id = 3,
            SectionType = SectionType.WhoWeSupport,
            Title = "Кого ми підтримуємо",
            CreatedAt = DateTime.Now,
            Contents = null!
        },
        new WhoWeAreSection
        {
            Id = 4,
            SectionType = SectionType.Team,
            Title = "Команда",
            CreatedAt = DateTime.Now,
            Contents = null!
        },
        new WhoWeAreSection
        {
            Id = 5,
            SectionType = SectionType.People,
            Title = "Люди",
            CreatedAt = DateTime.Now,
            Contents = null!
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
}
