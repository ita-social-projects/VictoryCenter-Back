using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        SetupDependencies(_settingsEntity, saveResult: 1);
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_settingsDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_settingsDto.ExchangeRate, result.Value.ExchangeRate);
    }

    [Fact]
    public async Task Handle_ShouldCreateSettings_WhenSettingsNotFound()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1);
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresSettingsConstants.SingletonSettingsId, result.Value.Id);
        Assert.Equal(string.Empty, result.Value.DisclaimerTitle);
        Assert.Equal(1m, result.Value.ExchangeRate);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFailsDuringSettingsCreation()
    {
        // Arrange
        SetupDependencies(null, saveResult: 0);
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresSettingsEntity)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccursDuringSettingsCreation()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1, saveException: new DbUpdateException());
        var handler = new GetReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new GetReportFundsExpendituresSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresSettingsEntity)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(
        ReportFundsExpendituresSettingsEntity? settings,
        int saveResult,
        Exception? saveException = null)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresSettingsRepository)
            .Returns(_settingsRepositoryMock.Object);

        _settingsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresSettingsEntity>>()))
            .ReturnsAsync(settings);

        _settingsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .ReturnsAsync((ReportFundsExpendituresSettingsEntity entity) => entity);

        if (saveException is null)
        {
            _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
        }
        else
        {
            _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(saveException);
        }

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresSettingsDto>(It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .Returns((ReportFundsExpendituresSettingsEntity entity) => new ReportFundsExpendituresSettingsDto
            {
                Id = entity.Id,
                DisclaimerTitle = entity.DisclaimerTitle,
                ExchangeRate = entity.ExchangeRate
            });
    }
}
