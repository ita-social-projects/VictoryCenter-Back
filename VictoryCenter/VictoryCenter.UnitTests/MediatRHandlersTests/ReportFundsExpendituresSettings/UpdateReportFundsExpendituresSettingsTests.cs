using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresSettingsRepository> _settingsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresSettingsLocalizationsRepository> _localizationsRepositoryMock;
    private readonly IValidator<UpdateReportFundsExpendituresSettingsCommand> _validator;

    private readonly UpdateReportFundsExpendituresSettingsDto _updateDto = new()
    {
        DisclaimerTitle = "Updated disclaimer",
        ExchangeRate = 40.123456m
    };

    private readonly ReportFundsExpendituresSettingsEntity _settingsEntity = new()
    {
        Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
        DisclaimerTitle = "Old disclaimer",
        ExchangeRate = 38.123456m
    };

    private readonly ReportFundsExpendituresSettingsDto _settingsDto = new()
    {
        Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
        DisclaimerTitle = "Updated disclaimer",
        ExchangeRate = 40.123456m
    };

    public UpdateReportFundsExpendituresSettingsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _settingsRepositoryMock = new Mock<IReportFundsExpendituresSettingsRepository>();
        _localizationsRepositoryMock = new Mock<IReportFundsExpendituresSettingsLocalizationsRepository>();
        _validator = new UpdateReportFundsExpendituresSettingsValidator(new BaseReportFundsExpendituresSettingsValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateSettings()
    {
        // Arrange
        SetupDependencies(_settingsEntity, saveResult: 1);
        var handler = new UpdateReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresSettingsCommand(_updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_settingsDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_settingsDto.ExchangeRate, result.Value.ExchangeRate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_WhenDisclaimerIsInvalid(string? disclaimer)
    {
        // Arrange
        var invalidDto = _updateDto with { DisclaimerTitle = disclaimer! };
        var handler = new UpdateReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresSettingsCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateAndUpdateSettings_WhenSettingsNotFound()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1);
        var handler = new UpdateReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresSettingsCommand(_updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_updateDto.DisclaimerTitle, result.Value.DisclaimerTitle);
        Assert.Equal(_updateDto.ExchangeRate, result.Value.ExchangeRate);
        _settingsRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<ReportFundsExpendituresSettingsEntity>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_settingsEntity, saveResult: 0);
        var handler = new UpdateReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresSettingsCommand(_updateDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresSettingsEntity)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(_settingsEntity, saveResult: 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = new UpdateReportFundsExpendituresSettingsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresSettingsCommand(_updateDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresSettingsEntity)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportFundsExpendituresSettingsEntity? settings, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresSettingsRepository)
            .Returns(_settingsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresSettingsLocalizationsRepository)
            .Returns(_localizationsRepositoryMock.Object);
        _localizationsRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresSettingsLocalization>>()))
            .ReturnsAsync(new List<ReportFundsExpendituresSettingsLocalization>());

        _settingsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresSettingsEntity>>()))
            .ReturnsAsync(settings);

        _settingsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .ReturnsAsync((ReportFundsExpendituresSettingsEntity entity) => entity);

        _settingsRepositoryMock.Setup(repository => repository.Update(It.IsAny<ReportFundsExpendituresSettingsEntity>()));
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mapperMock
            .Setup(mapper => mapper.Map(
                It.IsAny<UpdateReportFundsExpendituresSettingsDto>(),
                It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .Callback<UpdateReportFundsExpendituresSettingsDto, ReportFundsExpendituresSettingsEntity>(
                (dto, entity) =>
                {
                    entity.DisclaimerTitle = dto.DisclaimerTitle;
                    entity.ExchangeRate = dto.ExchangeRate;
                })
            .Returns((UpdateReportFundsExpendituresSettingsDto _, ReportFundsExpendituresSettingsEntity entity) => entity);

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresSettingsDto>(It.IsAny<ReportFundsExpendituresSettingsEntity>()))
            .Returns(_settingsDto);
    }
}
