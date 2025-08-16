using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Queries.Programs.GetByFilters;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class GetPrograms
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;
    private readonly Mock<IBlobService> _blobService;

    private readonly List<DAL.Entities.Program> _programs =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            CreatedAt = DateTime.UtcNow,
            ImageId = 1,
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Draft,
            CreatedAt = DateTime.UtcNow,
            ImageId = 2,
        },
        new()
        {
            Id = 3,
            Name = "TestName3",
            Description = "TestDescription3",
            Status = Status.Published,
            CreatedAt = DateTime.UtcNow,
            ImageId = 3,
        },
        new()
        {
            Id = 4,
            Name = "TestName4",
            Description = "TestDescription4",
            Status = Status.Draft,
            CreatedAt = DateTime.UtcNow,
            ImageId = 4,
        },
        new()
        {
            Id = 5,
            Name = "TestName5",
            Description = "TestDescription5",
            Status = Status.Published,
            CreatedAt = DateTime.UtcNow,
            ImageId = 5,
        }

    ];

    private readonly List<ProgramDto> _responseDto = new()
    {
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
        }
    };

    public GetPrograms()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapper = new Mock<IRepositoryWrapper>();
        _blobService = new Mock<IBlobService>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    public async Task Handle_ShouldReturnAllPrograms_NoFilters(int pageNumber, int pageLimit)
    {
        var programResponseDto = _responseDto
            .Skip(pageNumber * pageLimit)
            .Take(pageLimit)
            .ToList();

        var programEntities = _programs
            .Skip(pageNumber * pageLimit)
            .Take(pageLimit)
            .ToList();

        SetUpDependencies(programResponseDto, programEntities);

        var handler = new GetByFiltersHandler(_mockMapper.Object, _blobService.Object,  _repositoryWrapper.Object);

        ProgramFilterRequestDto requestDto = new()
        {
            Offset = pageNumber,
            Limit = pageLimit,
            Status = null,
            CategoryId = null
        };

        var result = await handler
            .Handle(new GetByFiltersQuery(requestDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(programResponseDto, result.Value.Programs);
    }

    [Theory]
    [InlineData(Status.Published)]
    [InlineData(Status.Draft)]
    public async Task Handle_ShouldFilterByStatus(Status status)
    {
        var programEntities = _programs.Where(p => p.Status == status).ToList();
        var programResponseDtos = _responseDto.Where(p => p.Status == status).ToList();

        SetUpDependencies(programResponseDtos, programEntities);

        ProgramFilterRequestDto requestDto = new()
        {
            Offset = 0,
            Status = status,
            CategoryId = null
        };

        var handler = new GetByFiltersHandler(_mockMapper.Object, _blobService.Object,  _repositoryWrapper.Object);
        var result = await handler.Handle(new GetByFiltersQuery(requestDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(programResponseDtos, result.Value.Programs);
    }

    private void SetUpDependencies(List<ProgramDto> responseDto, List<DAL.Entities.Program> programs)
    {
        SetUpBlobService();
        SetUpMapper(responseDto);
        SetUpRepositoryWrapper(programs);
    }

    private void SetUpMapper(List<ProgramDto> responseDto)
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<ProgramDto>>(It.IsAny<IEnumerable<DAL.Entities.Program>>()))
            .Returns(responseDto);
    }

    private void SetUpBlobService()
    {
        _blobService
            .Setup(x => x.FindFileInStorageAsBase64Async(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("mockedBase64");
    }

    private void SetUpRepositoryWrapper(List<DAL.Entities.Program> programs)
    {
        _repositoryWrapper.Setup(r => r.ProgramsRepository
                .GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.Program>>()))
            .ReturnsAsync(programs);
    }
}
