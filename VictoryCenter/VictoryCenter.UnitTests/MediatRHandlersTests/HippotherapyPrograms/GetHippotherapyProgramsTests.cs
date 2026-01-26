using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class GetHippotherapyProgramsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IImageRepository> _mockImageRepository;

    private readonly List<HippotherapyProgram> _programs =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            Sections = []
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
            Sections = []
        },
        new()
        {
            Id = 3,
            Name = "TestName3",
            Description = "TestDescription3",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            Sections = []
        },
        new()
        {
            Id = 4,
            Name = "TestName4",
            Description = "TestDescription4",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
            Sections = []
        },
        new()
        {
            Id = 5,
            Name = "TestName5",
            Description = "TestDescription5",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            Sections = []
        }

    ];

    private readonly List<HippotherapyProgramDto> _responseDto =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Draft
        },
        new()
        {
            Id = 3,
            Name = "TestName3",
            Description = "TestDescription3",
            Status = Status.Published
        },
        new()
        {
            Id = 4,
            Name = "TestName4",
            Description = "TestDescription4",
            Status = Status.Draft
        },
        new()
        {
            Id = 5,
            Name = "TestName5",
            Description = "TestDescription5",
            Status = Status.Published
        },
    ];

    public GetHippotherapyProgramsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockImageRepository = new Mock<IImageRepository>();

        _repositoryWrapper.Setup(r => r.ImageRepository).Returns(_mockImageRepository.Object);
    }

    [Theory]
    [InlineData(Status.Published)]
    [InlineData(Status.Draft)]
    public async Task Handle_ShouldFilterByStatus(Status status)
    {
        var programEntities = _programs.Where(p => p.Status == status).ToList();
        var programResponseDtos = _responseDto.Where(p => p.Status == status).ToList();

        SetUpDependencies(programResponseDtos, programEntities);

        HippotherapyProgramsFilterDto requestDto = new()
        {
            Offset = 0,
            Status = status,
            CategoryId = null
        };

        var handler = new GetHippotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetHippotherapyProgramsByFiltersQuery(requestDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(programResponseDtos, result.Value.Items);
    }

    [Fact]
    public async Task Handle_ShouldLoadImages_WhenProgramsHaveImageContent()
    {
        var image = new Image { Id = 1, BlobName = "test.jpg", MimeType = "image/jpeg" };
        var section = new HippotherapyProgramSection
        {
            Id = 1,
            ProgramId = 1,
            Contents = new List<ProgramSectionContent>
            {
                new ImageProgramContent { Id = 1, ImageId = 1, ContentType = ContentType.Image }
            }
        };

        var program = new HippotherapyProgram
        {
            Id = 1,
            Name = "Test",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section }
        };

        _repositoryWrapper.Setup(r => r.HippotherapyProgramsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(new List<HippotherapyProgram> { program });
        _repositoryWrapper.Setup(r => r.HippotherapyProgramsRepository.CountAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(1);
        _mockImageRepository.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(new List<Image> { image });
        _mockMapper.Setup(m => m.Map<IEnumerable<HippotherapyProgramDto>>(It.IsAny<IEnumerable<HippotherapyProgram>>()))
            .Returns(new List<HippotherapyProgramDto> { new() { Id = 1, Name = "Test" } });

        var handler = new GetHippotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetHippotherapyProgramsByFiltersQuery(new HippotherapyProgramsFilterDto()), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockImageRepository.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()), Times.Once);
        Assert.Equal(image, ((ImageProgramContent)section.Contents.First()).Image);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenImageNotFound()
    {
        var section = new HippotherapyProgramSection
        {
            Id = 1,
            Contents = new List<ProgramSectionContent>
            {
                new ImageProgramContent { Id = 1, ImageId = 999 }
            }
        };

        var program = new HippotherapyProgram { Id = 1, Sections = new List<HippotherapyProgramSection> { section } };

        _repositoryWrapper.Setup(r => r.HippotherapyProgramsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(new List<HippotherapyProgram> { program });
        _mockImageRepository.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(new List<Image>());

        var handler = new GetHippotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetHippotherapyProgramsByFiltersQuery(new HippotherapyProgramsFilterDto()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    private void SetUpDependencies(IEnumerable<HippotherapyProgramDto> responseDto, IEnumerable<HippotherapyProgram> programs)
    {
        SetUpMapper(responseDto);
        SetUpRepositoryWrapper(programs);
        SetUpImageRepository();
    }

    private void SetUpMapper(IEnumerable<HippotherapyProgramDto> responseDto)
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<HippotherapyProgramDto>>(It.IsAny<IEnumerable<HippotherapyProgram>>()))
            .Returns(responseDto);
    }

    private void SetUpRepositoryWrapper(IEnumerable<HippotherapyProgram> programs)
    {
        _repositoryWrapper.Setup(r => r.HippotherapyProgramsRepository
                .GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(programs);

        _repositoryWrapper.Setup(r => r.HippotherapyProgramsRepository
                .CountAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(programs.Count());
    }

    private void SetUpImageRepository()
    {
        _mockImageRepository.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(new List<Image>());
    }
}
