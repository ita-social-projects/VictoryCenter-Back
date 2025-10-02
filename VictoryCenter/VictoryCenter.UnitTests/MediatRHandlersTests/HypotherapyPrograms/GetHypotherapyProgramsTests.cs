using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetByFilters;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyPrograms;

public class GetHypotherapyProgramsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper;

    private readonly List<DAL.Entities.HypotherapyProgram> _programs =
    [
        new()
        {
            Id = 1,
            Name = "TestName1",
            Description = "TestDescription1",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageId = 1,
        },
        new()
        {
            Id = 2,
            Name = "TestName2",
            Description = "TestDescription2",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageId = 2,
        },
        new()
        {
            Id = 3,
            Name = "TestName3",
            Description = "TestDescription3",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageId = 3,
        },
        new()
        {
            Id = 4,
            Name = "TestName4",
            Description = "TestDescription4",
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageId = 4,
        },
        new()
        {
            Id = 5,
            Name = "TestName5",
            Description = "TestDescription5",
            Status = Status.Published,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageId = 5,
        }

    ];

    private readonly List<HypotherapyProgramDto> _responseDto =
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

    public GetHypotherapyProgramsTests()
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

        var handler = new GetHypotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);

        HypotherapyProgramsFilterDto requestDto = new()
        {
            Offset = pageNumber,
            Limit = pageLimit,
            Status = null,
            CategoryId = null
        };

        var result = await handler
            .Handle(new GetHypotherapyProgramsByFiltersQuery(requestDto), CancellationToken.None);

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

        HypotherapyProgramsFilterDto requestDto = new()
        {
            Offset = 0,
            Status = status,
            CategoryId = null
        };

        var handler = new GetHypotherapyProgramsByFiltersHandler(_mockMapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetHypotherapyProgramsByFiltersQuery(requestDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(programResponseDtos, result.Value.Items);
    }

    private void SetUpDependencies(IEnumerable<HypotherapyProgramDto> responseDto, IEnumerable<DAL.Entities.HypotherapyProgram> programs)
    {
        SetUpMapper(responseDto);
        SetUpRepositoryWrapper(programs);
    }

    private void SetUpMapper(IEnumerable<HypotherapyProgramDto> responseDto)
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<HypotherapyProgramDto>>(It.IsAny<IEnumerable<DAL.Entities.HypotherapyProgram>>()))
            .Returns(responseDto);
    }

    private void SetUpRepositoryWrapper(IEnumerable<DAL.Entities.HypotherapyProgram> programs)
    {
        _repositoryWrapper.Setup(r => r.HypotherapyProgramsRepository
                .GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.HypotherapyProgram>>()))
            .ReturnsAsync(programs);
    }
}
