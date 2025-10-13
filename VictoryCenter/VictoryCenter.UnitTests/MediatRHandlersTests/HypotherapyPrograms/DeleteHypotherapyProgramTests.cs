using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HypotherapyPrograms;

public class DeleteHypotherapyProgramTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly DAL.Entities.HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1
    };

    public DeleteHypotherapyProgramTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteHypotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHypotherapyProgramCommand(1), CancellationToken.None);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_ProgramNotFound()
    {
        SetUpDependencies();
        var handler = new DeleteHypotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHypotherapyProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new DeleteHypotherapyProgramHandler(_repositoryWrapperMock.Object);
        Result<long> result = await handler.Handle(new DeleteHypotherapyProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.HippotherapyProgram program = null!, int saveResult = 1)
    {
        SetUpRepositoryWrapper(saveResult, program);
    }

    private void SetUpRepositoryWrapper(int saveResult, DAL.Entities.HippotherapyProgram program)
    {
        _repositoryWrapperMock.Setup(r => r.HypotherapyProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.HippotherapyProgram>>())).ReturnsAsync(program);
        _repositoryWrapperMock.Setup(r => r.HypotherapyProgramsRepository.Delete(It.IsAny<DAL.Entities.HippotherapyProgram>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
