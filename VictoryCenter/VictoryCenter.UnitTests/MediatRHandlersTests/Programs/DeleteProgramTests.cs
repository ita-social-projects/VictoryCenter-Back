using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Programs.Delete;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Programs;

public class DeleteProgramTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly DAL.Entities.Program _programEntity = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        ImageId = 1
    };

    public DeleteProgramTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteProgram()
    {
        SetUpDependencies(_programEntity);
        var handler = new DeleteProgramHandler(_repositoryWrapperMock.Object);
        var result = await handler.Handle(new DeleteProgramCommand(1), CancellationToken.None);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_ProgramNotFound()
    {
        SetUpDependencies();
        var handler = new DeleteProgramHandler(_repositoryWrapperMock.Object);
        var result = await handler.Handle(new DeleteProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(Program)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailDelete_SaveFail()
    {
        SetUpDependencies(_programEntity, -1);
        var handler = new DeleteProgramHandler(_repositoryWrapperMock.Object);
        var result = await handler.Handle(new DeleteProgramCommand(1), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramConstants.FailedToDeleteProgram, result.Errors[0].Message);
    }

    private void SetUpDependencies(DAL.Entities.Program program = null, int saveResult = 1)
    {
        SetUpRepositoryWrapper(saveResult, program);
    }

    private void SetUpRepositoryWrapper(int saveResult, DAL.Entities.Program program)
    {
        _repositoryWrapperMock.Setup(r => r.ProgramsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.Program>>())).ReturnsAsync(program);
        _repositoryWrapperMock.Setup(r => r.ProgramsRepository.Delete(It.IsAny<DAL.Entities.Program>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
