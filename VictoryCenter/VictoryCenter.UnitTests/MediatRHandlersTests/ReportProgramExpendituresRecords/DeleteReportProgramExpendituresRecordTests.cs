using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class DeleteReportProgramExpendituresRecordTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly IValidator<DeleteReportProgramExpendituresRecordCommand> _validator;

    private readonly ReportProgramExpendituresRecord _record = new()
    {
        Id = 1,
        HippotherapyProgramCategoryId = 1,
        ReportingYear = 2025,
        AmountUah = 100,
        AmountUsd = 20
    };

    public DeleteReportProgramExpendituresRecordTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _validator = new DeleteReportProgramExpendituresRecordCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldDeleteRecord()
    {
        // Arrange
        SetupDependencies(_record, saveResult: 1);
        var handler = new DeleteReportProgramExpendituresRecordHandler(_repositoryWrapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new DeleteReportProgramExpendituresRecordCommand(_record.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_record.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var handler = new DeleteReportProgramExpendituresRecordHandler(_repositoryWrapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new DeleteReportProgramExpendituresRecordCommand(0),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportProgramExpendituresRecord.Id)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordNotFound()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1);
        var handler = new DeleteReportProgramExpendituresRecordHandler(_repositoryWrapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new DeleteReportProgramExpendituresRecordCommand(999),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999, typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_record, saveResult: 0);
        var handler = new DeleteReportProgramExpendituresRecordHandler(_repositoryWrapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new DeleteReportProgramExpendituresRecordCommand(_record.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(_record, saveResult: 1);
        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Database error"));
        var handler = new DeleteReportProgramExpendituresRecordHandler(_repositoryWrapperMock.Object, _validator);

        // Act
        var result = await handler.Handle(
            new DeleteReportProgramExpendituresRecordCommand(_record.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportProgramExpendituresRecord? record, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(record);

        _recordsRepositoryMock.Setup(repository => repository.Delete(It.IsAny<ReportProgramExpendituresRecord>()));
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
