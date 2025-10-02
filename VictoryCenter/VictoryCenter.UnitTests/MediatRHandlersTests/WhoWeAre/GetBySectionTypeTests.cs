using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetByType;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class GetWhoWeAreSectionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IMapper> _mapper;

    public GetWhoWeAreSectionHandlerTests()
    {
        _repositoryWrapper = new Mock<IRepositoryWrapper>();
        _mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSection_WhenSectionExists()
    {
        // Arrange
        var sectionType = SectionType.Main;
        var entity = GetEntity();
        var expectedDto = GetDto();

        SetupRepositoryWrapper(entity);
        SetupMapper(expectedDto);

        var handler = new GetWhoWeAreSectionHandler(_repositoryWrapper.Object, _mapper.Object);
        var query = new GetWhoWeAreSectionQuery(sectionType);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedDto.Title, result.Value.Title);
        Assert.Equal(expectedDto.SectionType, result.Value.SectionType);
        Assert.Equal(expectedDto.Contents.Count, result.Value.Contents.Count);
    }

    private void SetupRepositoryWrapper(WhoWeAreSection entity)
    {
        _repositoryWrapper
            .Setup(r => r.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(entity);
    }

    private void SetupMapper(WhoWeAreSectionDto dto)
    {
        _mapper
            .Setup(m => m.Map<WhoWeAreSectionDto>(It.IsAny<WhoWeAreSection>()))
            .Returns(dto);
    }

    private WhoWeAreSectionDto GetDto() => new()
    {
        Title = "Основне",
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
    };

    private WhoWeAreSection GetEntity() => new()
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
    };
}
