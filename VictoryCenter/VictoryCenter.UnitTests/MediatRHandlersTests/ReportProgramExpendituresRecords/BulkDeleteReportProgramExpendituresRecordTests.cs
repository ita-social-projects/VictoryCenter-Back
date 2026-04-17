using System.Transactions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class BulkDeleteReportProgramExpendituresRecordTests
{
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<BulkDeleteReportProgramExpendituresRecordCommand> _validator;

    public BulkDeleteReportProgramExpendituresRecordTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _validator = new BulkDeleteReportProgramExpendituresRecordCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldDeleteRecords()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportProgramExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);
        SetupDependencies(entities, 1);

        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ids, result.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldFail_WhenIdIsInvalid(long invalidId)
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand([invalidId]);
        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportProgramExpendituresRecord.Id)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsAreNotUnique()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand([1, 1]);
        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsAreEmpty()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(Array.Empty<long>());
        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsExceedMaximumCount()
    {
        // Arrange
        var maxCount = ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete;
        var ids = Enumerable.Range(1, maxCount + 1).Select(i => (long)i).ToArray();
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);
        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids), maxCount),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntitiesNotFound()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var existingIds = new List<long> { 1, 2 };
        var entities = existingIds.Select(id => new ReportProgramExpendituresRecord { Id = id }).ToList();
        var nonExistingIds = new List<long> { 3 };
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);
        SetupDependencies(entities, 0);

        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(nonExistingIds, typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportProgramExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);
        SetupDependencies(entities, 0);

        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntities(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportProgramExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);

        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _repositoryWrapperMock.Setup(wrapper => wrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _recordsRepositoryMock.Setup(repository =>
                repository.GetAllAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(entities);

        _recordsRepositoryMock.Setup(repository =>
            repository.DeleteRange(It.IsAny<IEnumerable<ReportProgramExpendituresRecord>>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new BulkDeleteReportProgramExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntitiesInDatabase(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(IEnumerable<ReportProgramExpendituresRecord> returnedEntities, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _repositoryWrapperMock.Setup(wrapper => wrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _recordsRepositoryMock.Setup(repository =>
                repository.GetAllAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(returnedEntities);

        _recordsRepositoryMock.Setup(repository =>
            repository.DeleteRange(It.IsAny<IEnumerable<ReportProgramExpendituresRecord>>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
