using Moq;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetSummary;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class GetReportFundsExpendituresSummaryTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;

    public GetReportFundsExpendituresSummaryTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSummary()
    {
        // Arrange
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetSummaryAsync())
            .ReturnsAsync((1000m, 250m, 4, 501m, 120m, 3));

        var handler = new GetReportFundsExpendituresSummaryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSummaryQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1000m, result.Value.IncomeUahTotal);
        Assert.Equal(250m, result.Value.IncomeUsdTotal);
        Assert.Equal(4, result.Value.IncomeCategoriesCount);
        Assert.Equal(501m, result.Value.ExpenditureUahTotal);
        Assert.Equal(120m, result.Value.ExpenditureUsdTotal);
        Assert.Equal(3, result.Value.ExpenditureCategoriesCount);
    }
}
