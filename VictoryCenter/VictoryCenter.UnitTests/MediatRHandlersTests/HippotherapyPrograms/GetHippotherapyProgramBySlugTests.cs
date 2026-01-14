using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetBySlug;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class GetHippotherapyProgramBySlugTests
{
    [Fact]
    public async Task Handle_WhenProgramNotFound_ShouldReturnFail()
    {
        var mapperMock = new Mock<IMapper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var slugServiceMock = new Mock<ISlugService>();

        slugServiceMock
            .Setup(s => s.GetHippotherapyProgramBySlugAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((HippotherapyProgram?)null);

        var handler = new GetHippotherapyProgramBySlugHandler(mapperMock.Object, repoWrapperMock.Object, slugServiceMock.Object);

        var result = await handler.Handle(new GetHippotherapyProgramBySlugQuery("missing"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WhenProgramIsNotPublished_ShouldReturnFail()
    {
        var mapperMock = new Mock<IMapper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var slugServiceMock = new Mock<ISlugService>();

        slugServiceMock
            .Setup(s => s.GetHippotherapyProgramBySlugAsync("draft", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HippotherapyProgram { Slug = "draft", Status = Status.Draft, Sections = new List<HippotherapyProgramSection>() });

        var handler = new GetHippotherapyProgramBySlugHandler(mapperMock.Object, repoWrapperMock.Object, slugServiceMock.Object);

        var result = await handler.Handle(new GetHippotherapyProgramBySlugQuery("draft"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WhenImageValidationFails_ShouldReturnFail()
    {
        var mapperMock = new Mock<IMapper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var slugServiceMock = new Mock<ISlugService>();
        var imageRepoMock = new Mock<IImageRepository>();

        repoWrapperMock.SetupGet(r => r.ImageRepository).Returns(imageRepoMock.Object);

        var program = new HippotherapyProgram
        {
            Slug = "test",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection>
            {
                new()
                {
                    Contents = new List<ProgramSectionContent>
                    {
                        new ImageProgramContent { ImageId = 111 },
                    },
                },
            },
        };

        slugServiceMock
            .Setup(s => s.GetHippotherapyProgramBySlugAsync("test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>() ))
            .ReturnsAsync(Array.Empty<Image>());

        var handler = new GetHippotherapyProgramBySlugHandler(mapperMock.Object, repoWrapperMock.Object, slugServiceMock.Object);

        var result = await handler.Handle(new GetHippotherapyProgramBySlugQuery("test"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldAssignImages_AndReturnMappedDto()
    {
        var mapperMock = new Mock<IMapper>();
        var repoWrapperMock = new Mock<IRepositoryWrapper>();
        var slugServiceMock = new Mock<ISlugService>();
        var imageRepoMock = new Mock<IImageRepository>();

        repoWrapperMock.SetupGet(r => r.ImageRepository).Returns(imageRepoMock.Object);

        var program = new HippotherapyProgram
        {
            Id = 10,
            Slug = "test",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection>
            {
                new()
                {
                    Contents = new List<ProgramSectionContent>
                    {
                        new ImageProgramContent { ImageId = 111 },
                        new ImageProgramContent { ImageId = 222 },
                    },
                },
            },
        };

        slugServiceMock
            .Setup(s => s.GetHippotherapyProgramBySlugAsync("test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        imageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>() ))
            .ReturnsAsync(new List<Image>
            {
                new() { Id = 111 },
                new() { Id = 222 },
            });

        var expectedDto = new HippotherapyProgramDto { Id = 10, Name = "dto" };

        mapperMock
            .Setup(m => m.Map<HippotherapyProgramDto>(program))
            .Returns(expectedDto);

        var handler = new GetHippotherapyProgramBySlugHandler(mapperMock.Object, repoWrapperMock.Object, slugServiceMock.Object);

        var result = await handler.Handle(new GetHippotherapyProgramBySlugQuery("test"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expectedDto, result.Value);

        var imageContents = program.Sections.SelectMany(s => s.Contents).OfType<ImageProgramContent>().ToList();
        Assert.All(imageContents, c => Assert.NotNull(c.Image));
        Assert.Contains(imageContents, c => c.Image!.Id == 111);
        Assert.Contains(imageContents, c => c.Image!.Id == 222);
    }
}
