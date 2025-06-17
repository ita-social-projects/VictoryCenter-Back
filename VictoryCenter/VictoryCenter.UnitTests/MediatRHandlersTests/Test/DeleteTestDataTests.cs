using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using VictoryCenter.BLL.Commands.Test.DeleteTestData;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Test;

public class DeleteTestDataTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly TestEntity _testEntityToDelete = new();

    public DeleteTestDataTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_DeleteTestData_SuccessfullyDelete()
    {
        SetupDependencies(_testEntityToDelete);
        var handler = new DeleteTestDataHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTestDataCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_DeleteTestData_EntityNotFound()
    {
        SetupDependencies(null);
        var handler = new DeleteTestDataHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTestDataCommand(-1), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_DeleteTestData_ShouldFail()
    {
        SetupDependencies(_testEntityToDelete, -1);
        var handler = new DeleteTestDataHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTestDataCommand(1), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    private void SetupDependencies(TestEntity? testEntityToDelete, int isSuccess = 1)
    {
        SetupRepositoryWrapper(testEntityToDelete, isSuccess);
    }

    private void SetupRepositoryWrapper(TestEntity? testEntityToDelete, int isSuccess)
    {
        _mockRepositoryWrapper.Setup(repository => repository.TestRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TestEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TestEntity>, IIncludableQueryable<TestEntity, object>>>()))
            .ReturnsAsync(testEntityToDelete);

        _mockRepositoryWrapper.Setup(repository => repository.TestRepository.Delete(It.IsAny<TestEntity>()));

        _mockRepositoryWrapper.Setup(repository => repository.SaveChangesAsync()).ReturnsAsync(isSuccess);
    }
}
