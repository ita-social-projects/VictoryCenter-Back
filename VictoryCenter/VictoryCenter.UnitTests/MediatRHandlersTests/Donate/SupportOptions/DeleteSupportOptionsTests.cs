using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.SupportOptions;

public class DeleteSupportOptionsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Entities.SupportOptions _supportOptionsEntity;

    public DeleteSupportOptionsTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _supportOptionsEntity = new Entities.SupportOptions
        {
            Id = 1,
            Name = "Option1",
            Value = "Value1"
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteSupportOptions_WhenEntityExists()
    {
        SetupEntityRetrieval(_supportOptionsEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteSupportOptionsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(
            new DeleteSupportOptionsCommand(_supportOptionsEntity.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_supportOptionsEntity.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntityNotFound()
    {
        SetupEntityRetrieval(null);
        var handler = new DeleteSupportOptionsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(
            new DeleteSupportOptionsCommand(99),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(99, typeof(Entities.SupportOptions)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupEntityRetrieval(_supportOptionsEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteSupportOptionsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(
            new DeleteSupportOptionsCommand(_supportOptionsEntity.Id),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.SupportOptions)),
            result.Errors[0].Message);
    }

    private void SetupEntityRetrieval(Entities.SupportOptions? entity)
    {
        _repositoryWrapperMock.Setup(r => r.SupportOptionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.SupportOptions>>()))
            .ReturnsAsync(entity);
    }
}
