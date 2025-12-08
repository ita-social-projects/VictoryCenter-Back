using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Partners.GetPartnerSections;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class GetPartnerSectionsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly List<PartnerSection> _partnerSectionEntities =
    [
        new()
        {
            Id = 1,
            Title = "Секція 1",
            Priority = 1,
            Partners =
            [
                new() { Id = 10, Description = "Партнер 1", Priority = 1, Image = new Image { Id = 100 } }
            ]
        },
        new()
        {
            Id = 2,
            Title = "Секція 2",
            Priority = 2,
            Partners =
            [
                new() { Id = 20, Description = "Партнер 2", Priority = 1, Image = new Image { Id = 200 } },
                new() { Id = 21, Description = "Партнер 3", Priority = 2, Image = new Image { Id = 201 } }
            ]
        }

    ];

    private readonly List<PartnersSectionDto> _partnerSectionDtos =
    [
        new()
        {
            Id = 1,
            Title = "Секція 1",
            Partners =
            [
                new() { Id = 10, Description = "Партнер 1", Image = new ImageDto { Id = 100 } }
            ]
        },
        new()
        {
            Id = 2,
            Title = "Секція 2",
            Partners =
            [
                new() { Id = 20, Description = "Партнер 2", Image = new ImageDto { Id = 200 } },
                new() { Id = 21, Description = "Партнер 3", Image = new ImageDto { Id = 201 } }
            ]
        }

    ];

    public GetPartnerSectionsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_NoSectionsExist_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<PartnerSection>();
        var emptyDtoList = new List<PartnersSectionDto>();

        SetupRepositoryWrapper(emptyList);
        SetupMapper(emptyDtoList);

        var query = new GetPartnerSectionsQuery();
        var handler = new GetPartnerSectionsHandler(_mockRepoWrapper.Object, _mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);

        _mockRepoWrapper.Verify(r => r.PartnerSectionsRepository.GetAllAsync(It.IsAny<QueryOptions<PartnerSection>>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PartnersSectionDto>>(It.IsAny<IEnumerable<PartnerSection>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SectionsExist_ShouldReturnOkWithMappedList()
    {
        // Arrange
        SetupRepositoryWrapper(_partnerSectionEntities);
        SetupMapper(_partnerSectionDtos);

        var query = new GetPartnerSectionsQuery();
        var handler = new GetPartnerSectionsHandler(_mockRepoWrapper.Object, _mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_partnerSectionDtos.Count, result.Value.Count());
        Assert.Equal(_partnerSectionDtos, result.Value);

        _mockRepoWrapper.Verify(r => r.PartnerSectionsRepository.GetAllAsync(It.IsAny<QueryOptions<PartnerSection>>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PartnersSectionDto>>(It.IsAny<IEnumerable<PartnerSection>>()), Times.Once);
    }

    private void SetupMapper(IEnumerable<PartnersSectionDto> dtosToReturn)
    {
        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<PartnersSectionDto>>(It.IsAny<IEnumerable<PartnerSection>>()))
            .Returns(dtosToReturn);
    }

    private void SetupRepositoryWrapper(IEnumerable<PartnerSection> entitiesToReturn)
    {
        _mockRepoWrapper.Setup(
            repoWrapper => repoWrapper.PartnerSectionsRepository.GetAllAsync(
                It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(entitiesToReturn);
    }
}
