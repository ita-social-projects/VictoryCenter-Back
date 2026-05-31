using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class BulkDeleteReportFundsExpendituresRecordTests
{
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<BulkDeleteReportFundsExpendituresRecordCommand> _validator;

    public BulkDeleteReportFundsExpendituresRecordTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _validator = new BulkDeleteReportFundsExpendituresRecordCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldDeleteRecords()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportFundsExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(ids);
        SetupDependencies(entities, 1);

        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
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
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(new[] { invalidId });
        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecord.Id)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsAreNotUnique()
    {
        // Arrange
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(new[] { 1L, 1L });
        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsAreEmpty()
    {
        // Arrange
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(Array.Empty<long>());
        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdsExceedMaximumCount()
    {
        // Arrange
        var maxCount = ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete;
        var ids = Enumerable.Range(1, maxCount + 1).Select(i => (long)i).ToArray();
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(ids);
        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids), maxCount),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntitiesNotFound()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var existingIds = new List<long> { 1, 2 };
        var entities = existingIds.Select(id => new ReportFundsExpendituresRecord { Id = id }).ToList();
        var nonExistingIds = new List<long> { 3 };
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(ids);
        SetupDependencies(entities, 0);

        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(nonExistingIds, typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportFundsExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(ids);
        SetupDependencies(entities, 0);

        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntities(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var entities = ids.Select(id => new ReportFundsExpendituresRecord { Id = id }).ToList();
        var command = new BulkDeleteReportFundsExpendituresRecordCommand(ids);

        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock.Setup(repository =>
                repository.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>() ))
            .ReturnsAsync(entities);

        _recordsRepositoryMock.Setup(repository =>
            repository.DeleteRange(It.IsAny<IEnumerable<ReportFundsExpendituresRecord>>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new BulkDeleteReportFundsExpendituresRecordCommandHandler(
            _validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntitiesInDatabase(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(IEnumerable<ReportFundsExpendituresRecord> returnedEntities, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock.Setup(repository =>
                repository.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>() ))
            .ReturnsAsync(returnedEntities);

        _recordsRepositoryMock.Setup(repository =>
            repository.DeleteRange(It.IsAny<IEnumerable<ReportFundsExpendituresRecord>>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
