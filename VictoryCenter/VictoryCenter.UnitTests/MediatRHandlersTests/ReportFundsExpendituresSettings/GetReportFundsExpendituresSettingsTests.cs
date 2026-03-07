using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresSettings.Get;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresSettings;

public class GetReportFundsExpendituresSettingsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresSettingsRepository> _settingsRepositoryMock;

    private readonly ReportFundsExpendituresSettingsEntity _settingsEntity = new()
    {
        Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
        DisclaimerTitle = "Valid disclaimer",
        ExchangeRate = 40.123456m
    };

    private readonly ReportFundsExpendituresSettingsDto _settingsDto = new()
    {
        Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
        DisclaimerTitle = "Valid disclaimer",
        ExchangeRate = 40.123456m
    };

    public GetReportFundsExpendituresSettingsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _settingsRepositoryMock = new Mock<IReportFundsExpendituresSettingsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSettings()
    {
        // Arrange
        SetupDependencies(_settingsEntity);
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_settingsDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_settingsDto.ExchangeRate, result.Value.ExchangeRate);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettingsNotFound()
    {
        // Arrange
        SetupDependencies(null);
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(
                ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
                typeof(ReportFundsExpendituresSettingsEntity)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportFundsExpendituresSettingsEntity? settings)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresSettingsRepository)
            .Returns(_settingsRepositoryMock.Object);

        _settingsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresSettingsEntity>>()))
            .ReturnsAsync(settings);

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresSettingsDto>(It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .Returns(_settingsDto);
    }
}
