using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class GetHippotherapyProgramsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;

    private readonly List<HippotherapyProgram> _programs =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow
        },
        new()
        {
            Id = 3,
            Name = "TestName3",
            Description = "TestDescription3",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow
        },
        new()
        {
            Id = 4,
            Name = "TestName4",
            Description = "TestDescription4",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow
        },
        new()
        {
            Id = 5,
            Name = "TestName5",
            Description = "TestDescription5",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow
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

        var handler = new GetHippotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);

        HippotherapyProgramsFilterDto requestDto = new()
        {
            Offset = pageNumber,
            Limit = pageLimit,
            Status = null,
            CategoryId = null
        };

        var result = await handler
            .Handle(new GetHippotherapyProgramsByFiltersQuery(requestDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(programResponseDto, result.Value.Items);
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

    private void SetUpDependencies(IEnumerable<HippotherapyProgramDto> responseDto, IEnumerable<HippotherapyProgram> programs)
    {
        SetUpMapper(responseDto);
        SetUpRepositoryWrapper(programs);
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
    }
}
