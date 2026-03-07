using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class DeleteReportFundsExpendituresRecordTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;

    private readonly ReportFundsExpendituresRecord _record = new()
    {
        Id = 1,
        CategoryId = 1,
        ReportingYear = 2025,
        AmountUah = 100,
        AmountUsd = 20
    };

    public DeleteReportFundsExpendituresRecordTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteRecord()
    {
        // Arrange
        SetupDependencies(_record, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresRecordHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresRecordCommand(_record.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_record.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordNotFound()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresRecordHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresRecordCommand(999),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999, typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_record, saveResult: 0);
        var handler = new DeleteReportFundsExpendituresRecordHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresRecordCommand(_record.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportFundsExpendituresRecord? record, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(record);

        _recordsRepositoryMock.Setup(repository => repository.Delete(It.IsAny<ReportFundsExpendituresRecord>()));
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
