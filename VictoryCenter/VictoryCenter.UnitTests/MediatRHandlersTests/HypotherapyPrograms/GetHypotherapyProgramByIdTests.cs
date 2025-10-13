using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyPrograms;

public class GetHypotherapyProgramByIdTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly DAL.Entities.HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1,
    };

    private readonly HypotherapyProgramDto _programDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Image = new ImageDto()
    };

    public GetHypotherapyProgramByIdTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldFindProgram()
    {
        SetUpDependencies(_programEntity);
        var handler =
            new GetHypotherapyProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new GetHypotherapyProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_programDto.Name, result.Value.Name);
        Assert.Equal(_programDto.Description, result.Value.Description);
        Assert.Equal(_programDto.Status, result.Value.Status);

        _mockRepositoryWrapper.Verify(x => x.HypotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.HippotherapyProgram>>()), Times.Once);
        _mapperMock.Verify(x => x.Map<HypotherapyProgramDto>(It.IsAny<DAL.Entities.HippotherapyProgram>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFailFindProgram()
    {
        SetUpDependencies();
        var handler =
            new GetHypotherapyProgramByIdHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);
        Result<HypotherapyProgramDto> result = await handler.Handle(new GetHypotherapyProgramByIdQuery(_programEntity.Id), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(_programEntity.Id, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.HippotherapyProgram program = null!)
    {
        SetUpAutoMapper();
        SetUpRepositoryWrapper(program);
    }

    private void SetUpAutoMapper()
    {
        _mapperMock.Setup(x => x.Map<HypotherapyProgramDto>(It.IsAny<DAL.Entities.HippotherapyProgram>())).Returns(_programDto);
    }

    private void SetUpRepositoryWrapper(DAL.Entities.HippotherapyProgram program)
    {
        _mockRepositoryWrapper.Setup(x => x.HypotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.HippotherapyProgram>>())).ReturnsAsync(program);
    }
}
