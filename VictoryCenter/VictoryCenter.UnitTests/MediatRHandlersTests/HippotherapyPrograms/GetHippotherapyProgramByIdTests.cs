using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class GetHippotherapyProgramByIdTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft
    };

    private readonly HippotherapyProgramDto _programDto = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft
    };

    public GetHippotherapyProgramByIdTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldFindProgram()
    {
        SetUpDependencies(_programEntity);
        var handler =
            new GetHippotherapyProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);
        Result<HippotherapyProgramDto> result = await handler.Handle(new GetHippotherapyProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_programDto.Name, result.Value.Name);
        Assert.Equal(_programDto.Description, result.Value.Description);
        Assert.Equal(_programDto.Status, result.Value.Status);

        _mockRepositoryWrapper.Verify(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()), Times.Once);
        _mapperMock.Verify(x => x.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFailFindProgram()
    {
        SetUpDependencies();
        var handler =
            new GetHippotherapyProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);
        Result<HippotherapyProgramDto> result = await handler.Handle(new GetHippotherapyProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(_programEntity.Id, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(HippotherapyProgram program = null!)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(program);
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>())).Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(HippotherapyProgram program)
    {
        _mockRepositoryWrapper.Setup(x => x.HippotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>())).ReturnsAsync(program);
    }
}
